using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.CodeRefactoringProviders;

internal abstract class AbstractIntroduceSubstituteCodeRefactoringProvider<TObjectCreationExpressionSyntax, TArgumentListSyntax, TArgumentSyntax> : CodeRefactoringProvider
    where TObjectCreationExpressionSyntax : SyntaxNode
    where TArgumentListSyntax : SyntaxNode
    where TArgumentSyntax : SyntaxNode
{
    public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var node = root.FindNode(context.Span);

        if (node is not TArgumentListSyntax argumentListSyntax ||
            node.Parent is not TObjectCreationExpressionSyntax objectCreationExpressionSyntax)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync();
        var refactoringActions = CreateRefactoringActions(
            context,
            semanticModel,
            objectCreationExpressionSyntax,
            argumentListSyntax);

        foreach (var refactoringAction in refactoringActions)
        {
            context.RegisterRefactoring(refactoringAction);
        }
    }

    protected abstract IReadOnlyList<TArgumentSyntax> GetArgumentSyntaxNodes(TArgumentListSyntax argumentListSyntax, TextSpan span);

    protected abstract TObjectCreationExpressionSyntax UpdateObjectCreationExpression(
        TObjectCreationExpressionSyntax objectCreationExpressionSyntax,
        IReadOnlyList<TArgumentSyntax> argumentSyntax);

    protected virtual bool IsMissing(TArgumentSyntax argumentSyntax) => argumentSyntax.IsMissing;

    protected abstract SyntaxNode? FindSiblingNodeForLocalSubstitute(TObjectCreationExpressionSyntax creationExpression);

    protected abstract SyntaxNode? FindSiblingNodeForReadonlySubstitute(SyntaxNode creationExpression);

    private IEnumerable<CodeAction> CreateRefactoringActions(
        CodeRefactoringContext context,
        SemanticModel semanticModel,
        TObjectCreationExpressionSyntax objectCreationExpressionSyntax,
        TArgumentListSyntax argumentListSyntax)
    {
        var constructorSymbol = GetKnownConstructorSymbol(semanticModel, objectCreationExpressionSyntax);

        if (constructorSymbol == null || constructorSymbol.Parameters.Length == 0)
        {
            yield break;
        }

        var existingArguments = GetArgumentSyntaxNodes(argumentListSyntax, context.Span);
        var constructorParameters = constructorSymbol.Parameters.OrderBy(parameter => parameter.Ordinal).ToList();

        var missingArgumentsPositions = GetMissingArgumentsPositions(existingArguments, constructorParameters);
        if (missingArgumentsPositions.Count == 0)
        {
            yield break;
        }

        var localSubstituteSiblingNode = FindSiblingNodeForLocalSubstitute(objectCreationExpressionSyntax);
        var readonlySubstituteSiblingNode = FindSiblingNodeForReadonlySubstitute(objectCreationExpressionSyntax);

        var argumentIndexAtSpan = FindArgumentIndexAtSpan(existingArguments, context.Span);
        if (missingArgumentsPositions.Contains(argumentIndexAtSpan))
        {
            var substituteName = constructorParameters[argumentIndexAtSpan].ToMinimalSymbolString(semanticModel);

            if (localSubstituteSiblingNode != null)
            {
                yield return CodeAction.Create(
                    $"Introduce local substitute for {substituteName}",
                    _ => IntroduceLocalSubstitute(
                        context,
                        objectCreationExpressionSyntax,
                        existingArguments,
                        constructorParameters,
                        new[] { argumentIndexAtSpan },
                        localSubstituteSiblingNode));
            }

            if (readonlySubstituteSiblingNode != null)
            {
                yield return CodeAction.Create(
                    $"Introduce readonly substitute for {substituteName}",
                    _ => IntroduceReadonlySubstitute(
                        context,
                        objectCreationExpressionSyntax,
                        existingArguments,
                        constructorParameters,
                        new[] { argumentIndexAtSpan },
                        readonlySubstituteSiblingNode));
            }
        }

        if (localSubstituteSiblingNode != null)
        {
            yield return CodeAction.Create(
                "Introduce local substitutes for missing arguments",
                token => IntroduceLocalSubstitute(
                    context,
                    objectCreationExpressionSyntax,
                    existingArguments,
                    constructorParameters,
                    missingArgumentsPositions,
                    localSubstituteSiblingNode));
        }

        if (readonlySubstituteSiblingNode != null)
        {
            yield return CodeAction.Create(
                "Introduce readonly substitutes for missing arguments",
                _ => IntroduceReadonlySubstitute(
                    context,
                    objectCreationExpressionSyntax,
                    existingArguments,
                    constructorParameters,
                    missingArgumentsPositions,
                    readonlySubstituteSiblingNode));
        }
    }

    private async Task<Document> IntroduceReadonlySubstitute(
        CodeRefactoringContext context,
        TObjectCreationExpressionSyntax objectCreationExpressionSyntax,
        IReadOnlyList<TArgumentSyntax> existingArguments,
        IReadOnlyList<IParameterSymbol> constructorParameters,
        IReadOnlyList<int> missingArgumentsPositions,
        SyntaxNode siblingNode)
    {
        SyntaxNode CreateFieldDeclaration(SyntaxGenerator syntaxGenerator, IParameterSymbol parameterSymbol, SyntaxNode invocationExpression)
        {
            return syntaxGenerator.FieldDeclaration(
                parameterSymbol.Name,
                syntaxGenerator.TypeExpression(parameterSymbol.Type),
                Accessibility.Private,
                DeclarationModifiers.ReadOnly,
                invocationExpression);
        }

        return await IntroduceSubstitute(
            context,
            objectCreationExpressionSyntax,
            existingArguments,
            constructorParameters,
            missingArgumentsPositions,
            siblingNode,
            CreateFieldDeclaration);
    }

    private async Task<Document> IntroduceLocalSubstitute(
        CodeRefactoringContext context,
        TObjectCreationExpressionSyntax objectCreationExpressionSyntax,
        IReadOnlyList<TArgumentSyntax> existingArguments,
        IReadOnlyList<IParameterSymbol> constructorParameters,
        IReadOnlyList<int> missingArgumentsPositions,
        SyntaxNode siblingNode)
    {
        SyntaxNode CreateLocalDeclaration(SyntaxGenerator syntaxGenerator, IParameterSymbol parameterSymbol, SyntaxNode invocationExpression)
        {
            return syntaxGenerator.LocalDeclarationStatement(parameterSymbol.Name, invocationExpression);
        }

        return await IntroduceSubstitute(
            context,
            objectCreationExpressionSyntax,
            existingArguments,
            constructorParameters,
            missingArgumentsPositions,
            siblingNode,
            CreateLocalDeclaration);
    }

    private async Task<Document> IntroduceSubstitute(
        CodeRefactoringContext context,
        TObjectCreationExpressionSyntax objectCreationExpressionSyntax,
        IReadOnlyList<TArgumentSyntax> existingArguments,
        IReadOnlyList<IParameterSymbol> constructorParameters,
        IReadOnlyList<int> missingArgumentsPositions,
        SyntaxNode siblingNode,
        Func<SyntaxGenerator, IParameterSymbol, SyntaxNode, SyntaxNode> declarationFactory)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document);
        var syntaxGenerator = documentEditor.Generator;
        var declarations = new List<SyntaxNode>();
        var newArgumentList = new List<TArgumentSyntax>();

        for (var parameterPosition = 0; parameterPosition < constructorParameters.Count; parameterPosition++)
        {
            var parameterSymbol = constructorParameters[parameterPosition];
            if (missingArgumentsPositions.Contains(parameterPosition))
            {
                var invocationExpression = syntaxGenerator.SubstituteForInvocationExpression(parameterSymbol);

                var declaration = declarationFactory(
                    syntaxGenerator,
                    parameterSymbol,
                    invocationExpression);

                declarations.Add(declaration);

                newArgumentList.Add((TArgumentSyntax)syntaxGenerator.Argument(syntaxGenerator.IdentifierName(parameterSymbol.Name)));
            }
            else if (parameterPosition < existingArguments.Count)
            {
                newArgumentList.Add(existingArguments[parameterPosition]);
            }
        }

        documentEditor.InsertBefore(siblingNode, declarations);

        documentEditor.ReplaceNode(
            objectCreationExpressionSyntax,
            UpdateObjectCreationExpression(objectCreationExpressionSyntax, newArgumentList));

        return documentEditor.GetChangedDocument();
    }

    private IMethodSymbol? GetKnownConstructorSymbol(SemanticModel semanticModel, TObjectCreationExpressionSyntax objectCreationExpressionSyntax)
    {
        var symbol = semanticModel.GetSymbolInfo(objectCreationExpressionSyntax);

        if (symbol.Symbol is IMethodSymbol methodSymbol)
        {
            return methodSymbol;
        }

        var candidateMethodSymbols = symbol.CandidateSymbols.OfType<IMethodSymbol>().ToList();

        if (candidateMethodSymbols.Count == 0 || candidateMethodSymbols.Count > 1)
        {
            return null;
        }

        return candidateMethodSymbols.Single();
    }

    private IReadOnlyList<int> GetMissingArgumentsPositions(
        IReadOnlyList<TArgumentSyntax> argumentSyntaxNodes,
        IReadOnlyList<IParameterSymbol> parameterSymbols)
    {
        if (parameterSymbols.Count == 0)
        {
            return Array.Empty<int>();
        }

        if (argumentSyntaxNodes.Count == 0)
        {
            return Enumerable.Range(0, parameterSymbols.Count).ToList();
        }

        var result = new List<int>();
        for (var symbolPosition = 0; symbolPosition < parameterSymbols.Count; symbolPosition++)
        {
            if (symbolPosition >= argumentSyntaxNodes.Count || IsMissing(argumentSyntaxNodes[symbolPosition]))
            {
                result.Add(symbolPosition);
            }
        }

        return result;
    }

    private int FindArgumentIndexAtSpan(IReadOnlyList<TArgumentSyntax> argumentSyntaxNodes, TextSpan span)
    {
        var findArgumentIndexAtSpan = argumentSyntaxNodes.IndexOf(node => node.FullSpan.IntersectsWith(span));
        return findArgumentIndexAtSpan >= 0 ? findArgumentIndexAtSpan : 0;
    }
}