using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractReEntrantSetupCodeFixProvider<TArgumentListSyntax, TArgumentSyntax> : CodeFixProvider
    where TArgumentListSyntax : SyntaxNode
    where TArgumentSyntax : SyntaxNode
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.ReEntrantSubstituteCall);

    protected abstract TArgumentSyntax CreateUpdatedArgumentSyntaxNode(TArgumentSyntax argumentSyntaxNode);

    protected abstract TArgumentSyntax CreateUpdatedParamsArgumentSyntaxNode(SyntaxGenerator syntaxGenerator, ITypeSymbol returnedTypeSymbol, TArgumentSyntax argumentSyntaxNode);

    protected abstract SyntaxNode GetArgumentExpressionSyntax(TArgumentSyntax argumentSyntax);

    protected abstract IEnumerable<SyntaxNode> GetParameterExpressionsFromArrayArgument(TArgumentSyntax argumentSyntaxNode);

    protected abstract int AwaitExpressionRawKind { get; }

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.FirstOrDefault(diag =>
            diag.Descriptor.Id == DiagnosticIdentifiers.ReEntrantSubstituteCall);

        if (diagnostic == null)
        {
            return;
        }

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
        var semanticModel = await context.Document.GetSemanticModelAsync();

        var argumentList = GetArgumentListSyntax(node);
        var allArguments = GetArguments(argumentList).ToList();

        if (IsFixSupported(semanticModel, allArguments) == false)
        {
            return;
        }

        var codeAction = CodeAction.Create(
            "Replace with lambda",
            ct => CreateChangedDocument(context, argumentList, allArguments, ct),
            nameof(AbstractReEntrantSetupCodeFixProvider<TArgumentListSyntax, TArgumentSyntax>));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    protected abstract IEnumerable<TArgumentSyntax> GetArguments(TArgumentListSyntax argumentSyntax);

    private async Task<Document> CreateChangedDocument(
        CodeFixContext context,
        TArgumentListSyntax argumentListSyntax,
        IReadOnlyList<TArgumentSyntax> argumentSyntaxes,
        CancellationToken ct)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, ct);
        var semanticModel = await context.Document.GetSemanticModelAsync(ct);
        var invocationSyntaxNode = argumentListSyntax.Parent;
        if (semanticModel.GetSymbolInfo(invocationSyntaxNode).Symbol is not IMethodSymbol methodSymbol)
        {
            return context.Document;
        }

        var skip = methodSymbol.MethodKind == MethodKind.Ordinary
            ? 1
            : 0;

        ITypeSymbol lambdaType = null;
        foreach (var argumentSyntax in argumentSyntaxes.Skip(skip))
        {
            if (IsArrayParamsArgument(semanticModel, argumentSyntax))
            {
                lambdaType ??= ConstructCallInfoLambdaType(methodSymbol, semanticModel.Compilation);
                var updatedParamsArgumentSyntaxNode = CreateUpdatedParamsArgumentSyntaxNode(
                    SyntaxGenerator.GetGenerator(context.Document),
                    lambdaType,
                    argumentSyntax);

                documentEditor.ReplaceNode(argumentSyntax, updatedParamsArgumentSyntaxNode);
            }
            else
            {
                var updatedArgumentSyntax = CreateUpdatedArgumentSyntaxNode(argumentSyntax);

                documentEditor.ReplaceNode(argumentSyntax, updatedArgumentSyntax);
            }
        }

        return await Simplifier.ReduceAsync(documentEditor.GetChangedDocument(), cancellationToken: ct);
    }

    private static ITypeSymbol ConstructCallInfoLambdaType(IMethodSymbol methodSymbol, Compilation compilation)
    {
        var callInfoOverloadMethodSymbol = methodSymbol.ContainingType.GetMembers(methodSymbol.Name)
            .Where(symbol => !symbol.Equals(methodSymbol.ConstructedFrom))
            .OfType<IMethodSymbol>()
            .First(method => method.Parameters.Any(param => param.Type.IsCallInfoDelegate(compilation)));

        var typeArgument = methodSymbol.TypeArguments.FirstOrDefault() ?? methodSymbol.ReceiverType;
        var constructedOverloadSymbol = callInfoOverloadMethodSymbol.Construct(typeArgument);
        var lambdaType = constructedOverloadSymbol.Parameters
            .First(param => param.Type.IsCallInfoDelegate(compilation)).Type;

        return lambdaType;
    }

    private bool IsFixSupported(SemanticModel semanticModel, IEnumerable<TArgumentSyntax> arguments)
    {
        return arguments.All(arg =>
        {
            var expressionSyntax = GetArgumentExpressionSyntax(arg);
            var arrayExpressions = GetParameterExpressionsFromArrayArgument(arg);

            return expressionSyntax.RawKind != AwaitExpressionRawKind &&
                   (IsArrayParamsArgument(semanticModel, arg) == false || (arrayExpressions != null && arrayExpressions.All(exp => exp.RawKind != AwaitExpressionRawKind)));
        });
    }

    private bool IsArrayParamsArgument(SemanticModel semanticModel, TArgumentSyntax argumentSyntax)
    {
        var operation = semanticModel.GetOperation(argumentSyntax);
        return operation is IArgumentOperation argumentOperation && argumentOperation.Parameter.IsParams;
    }

    private static TArgumentListSyntax GetArgumentListSyntax(SyntaxNode diagnosticNode)
    {
        var argumentListSyntax = diagnosticNode.Ancestors().OfType<TArgumentListSyntax>().FirstOrDefault();
        return argumentListSyntax;
    }
}