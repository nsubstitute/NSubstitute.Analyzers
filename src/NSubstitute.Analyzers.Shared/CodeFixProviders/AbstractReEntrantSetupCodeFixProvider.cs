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

internal abstract class AbstractReEntrantSetupCodeFixProvider<TArgumentSyntax> : CodeFixProvider
    where TArgumentSyntax : SyntaxNode
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.ReEntrantSubstituteCall);

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.FirstOrDefault(diag =>
            diag.Descriptor.Id == DiagnosticIdentifiers.ReEntrantSubstituteCall);

        if (diagnostic == null)
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync();
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

        if (semanticModel.GetOperation(node) is not { } nodeOperation)
        {
           return;
        }

        var invocationOperation = nodeOperation.Ancestors().OfType<IInvocationOperation>().FirstOrDefault();

        if (invocationOperation == null)
        {
           return;
        }

        if (semanticModel.GetSymbolInfo(invocationOperation.Syntax).Symbol is not IMethodSymbol methodSymbol)
        {
           return;
        }

        if (IsFixSupported(invocationOperation) == false)
        {
            return;
        }

        var codeAction = CodeAction.Create(
            "Replace with lambda",
            ct => CreateChangedDocument(context, semanticModel, invocationOperation, methodSymbol, ct),
            nameof(AbstractReEntrantSetupCodeFixProvider<TArgumentSyntax>));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    protected abstract string LambdaParameterName { get; }

    protected abstract IReadOnlyList<TArgumentSyntax> GetArguments(IInvocationOperation invocationOperation);

    protected abstract TArgumentSyntax UpdateArgumentExpression(TArgumentSyntax argument, SyntaxNode expression);

    protected abstract SyntaxNode GetArgumentExpression(TArgumentSyntax argument);

    // syntaxGenerator.ArrayCreationExpression() generates invalid syntax in current version of Roslyn, so we have to
    // generate array expression per language on our own
    protected abstract SyntaxNode CreateArrayCreationExpression(SyntaxNode typeSyntax, IEnumerable<SyntaxNode> elements);

    private async Task<Document> CreateChangedDocument(
        CodeFixContext context,
        SemanticModel semanticModel,
        IInvocationOperation invocationOperation,
        IMethodSymbol methodSymbol,
        CancellationToken ct)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, ct);
        var syntaxGenerator = SyntaxGenerator.GetGenerator(context.Document);

        // we cant use syntax directly from invocationOperation.Arguments because for params expression
        // passed as separated values, operation syntax is not ArgumentSyntax
        var arguments = GetArguments(invocationOperation);
        foreach (var (argumentSyntax, argumentOperation) in arguments.GroupJoin(
                     invocationOperation.Arguments,
                     argument => argument,
                     operation => operation.Syntax,
                     (argument, argumentOperation) => (argument, argumentOperation.SingleOrDefault())))
        {
            if (methodSymbol.MethodKind == MethodKind.Ordinary &&
                argumentOperation is not null &&
                argumentOperation.Parameter.Ordinal == 0)
            {
                continue;
            }

            if (IsArrayParamsArgument(argumentOperation))
            {
                var updatedParamsArgumentSyntaxNode = CreateUpdatedParamsArgument(
                    semanticModel,
                    methodSymbol,
                    argumentOperation,
                    syntaxGenerator,
                    argumentSyntax);

                documentEditor.ReplaceNode(argumentSyntax, updatedParamsArgumentSyntaxNode);
            }
            else
            {
                var updatedArgumentSyntax = CreateUpdatedArgument(syntaxGenerator, argumentSyntax);

                documentEditor.ReplaceNode(argumentSyntax, updatedArgumentSyntax);
            }
        }

        return await Simplifier.ReduceAsync(documentEditor.GetChangedDocument(), cancellationToken: ct);
    }

    private TArgumentSyntax CreateUpdatedParamsArgument(
        SemanticModel semanticModel,
        IMethodSymbol methodSymbol,
        IArgumentOperation argumentOperation,
        SyntaxGenerator syntaxGenerator,
        TArgumentSyntax argumentSyntax)
    {
        var lambdaType = ConstructCallInfoLambdaType(methodSymbol, semanticModel.Compilation);
        var lambdaTypeSyntax = syntaxGenerator.TypeExpression(lambdaType);
        var arrayElements = argumentOperation.Value.GetArrayElementValues()
            .Select(operation => CreateLambdaExpression(syntaxGenerator, operation.Syntax));
        var arrayCreationExpression =
            CreateArrayCreationExpression(lambdaTypeSyntax, arrayElements);

        return UpdateArgumentExpression(argumentSyntax, arrayCreationExpression);
    }

    private TArgumentSyntax CreateUpdatedArgument(SyntaxGenerator syntaxGenerator, TArgumentSyntax argument)
    {
        var expression = GetArgumentExpression(argument);
        var lambdaExpression = CreateLambdaExpression(syntaxGenerator, expression);

        return UpdateArgumentExpression(argument, lambdaExpression);
    }

    private static ITypeSymbol ConstructCallInfoLambdaType(IMethodSymbol methodSymbol, Compilation compilation)
    {
        var callInfoOverloadMethodSymbol = methodSymbol.ContainingType.GetMembers(methodSymbol.Name)
            .Where(symbol => !symbol.Equals(methodSymbol.ConstructedFrom))
            .OfType<IMethodSymbol>()
            .First(method => method.Parameters.Any(param => param.Type.IsCallInfoDelegate(compilation)));

        var typeArgument = methodSymbol.TypeArguments.FirstOrDefault() ?? methodSymbol.ReceiverType;
        var constructedOverloadSymbol = callInfoOverloadMethodSymbol.Construct(typeArgument);

        return constructedOverloadSymbol.Parameters
            .First(param => param.Type.IsCallInfoDelegate(compilation)).Type;
    }

    private bool IsFixSupported(IInvocationOperation invocationOperation)
    {
        return invocationOperation.Arguments.All(argumentOperation =>
        {
            if (argumentOperation.Value is IAwaitOperation)
            {
                return false;
            }

            var arrayValues = argumentOperation.Value.GetArrayElementValues();
            return IsArrayParamsArgument(argumentOperation) == false || (arrayValues != null && arrayValues.All(exp => exp is not IAwaitOperation));
        });
    }

    private bool IsArrayParamsArgument(IArgumentOperation operation) =>
        operation != null && operation.Parameter.IsParams;

    private SyntaxNode CreateLambdaExpression(SyntaxGenerator syntaxGenerator, SyntaxNode statement) =>
        syntaxGenerator.ValueReturningLambdaExpression(LambdaParameterName, statement);
}