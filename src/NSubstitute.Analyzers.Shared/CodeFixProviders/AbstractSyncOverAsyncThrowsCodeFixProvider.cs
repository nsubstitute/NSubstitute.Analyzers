using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractSyncOverAsyncThrowsCodeFixProvider<TInvocationExpressionSyntax> : CodeFixProvider
    where TInvocationExpressionSyntax : SyntaxNode
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIdentifiers.SyncOverAsyncThrows);

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic =
            context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SyncOverAsyncThrows);

        if (diagnostic == null)
        {
            return;
        }

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        if (!(root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is
                TInvocationExpressionSyntax invocation))
        {
            return;
        }

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        var methodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(invocation).Symbol;

        if (methodSymbol.Parameters.Any(param => param.Type.IsCallInfoDelegate(semanticModel)))
        {
            return;
        }

        var replacementMethod = methodSymbol.IsThrowsForAnyArgsMethod()
            ? "ReturnsForAnyArgs"
            : "Returns";

        var codeAction = CodeAction.Create(
            $"Replace with {replacementMethod}",
            ct => CreateChangedDocument(context, semanticModel, invocation, methodSymbol, ct),
            nameof(AbstractSyncOverAsyncThrowsCodeFixProvider<TInvocationExpressionSyntax>));

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    protected abstract SyntaxNode GetExpression(TInvocationExpressionSyntax invocationExpressionSyntax);

    private async Task<Document> CreateChangedDocument(
        CodeFixContext context,
        SemanticModel semanticModel,
        TInvocationExpressionSyntax currentInvocationExpression,
        IMethodSymbol invocationSymbol,
        CancellationToken cancellationToken)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
        var invocationOperation = (IInvocationOperation)semanticModel.GetOperation(currentInvocationExpression);

        var updatedInvocationExpression = await CreateUpdatedInvocationExpression(
            currentInvocationExpression,
            invocationOperation,
            invocationSymbol,
            context);

        documentEditor.ReplaceNode(currentInvocationExpression, updatedInvocationExpression);

        return documentEditor.GetChangedDocument();
    }

    private async Task<SyntaxNode> CreateUpdatedInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        IInvocationOperation invocationOperation,
        IMethodSymbol invocationSymbol,
        CodeFixContext context)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document);
        var syntaxGenerator = documentEditor.Generator;

        var fromExceptionInvocationExpression =
            CreateFromExceptionInvocationExpression(syntaxGenerator, invocationOperation);

        var returnsMethodName =
            invocationSymbol.IsThrowsForAnyArgsMethod() ? "ReturnsForAnyArgs" : "Returns";

        if (invocationSymbol.MethodKind == MethodKind.Ordinary)
        {
            return CreateUpdatedOrdinalInvocationExpression(
                currentInvocationExpression,
                invocationOperation,
                syntaxGenerator,
                fromExceptionInvocationExpression,
                returnsMethodName);
        }

        return CreateUpdatedExtensionInvocationExpression(
            currentInvocationExpression,
            syntaxGenerator,
            fromExceptionInvocationExpression,
            returnsMethodName);
    }

    private static SyntaxNode CreateUpdatedOrdinalInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        IInvocationOperation invocationOperation,
        SyntaxGenerator syntaxGenerator,
        SyntaxNode fromExceptionInvocationExpression,
        string returnsMethodName)
    {
        return syntaxGenerator.InvocationExpression(
            syntaxGenerator.MemberAccessExpression(
                syntaxGenerator.DottedName("NSubstitute.SubstituteExtensions"), returnsMethodName),
            invocationOperation.Arguments.First(arg => arg.Parameter.Ordinal == 0).Value.Syntax,
            fromExceptionInvocationExpression).WithTriviaFrom(currentInvocationExpression);
    }

    private SyntaxNode CreateUpdatedExtensionInvocationExpression(
        TInvocationExpressionSyntax currentInvocationExpression,
        SyntaxGenerator syntaxGenerator,
        SyntaxNode fromExceptionInvocationExpression,
        string returnsMethodName)
    {
        var expressionSyntax = GetExpression(currentInvocationExpression);

        var accessExpression =
            syntaxGenerator.MemberAccessExpression(expressionSyntax, returnsMethodName);

        return syntaxGenerator.InvocationExpression(accessExpression, fromExceptionInvocationExpression)
            .WithTriviaFrom(currentInvocationExpression);
    }

    private static SyntaxNode CreateFromExceptionInvocationExpression(
        SyntaxGenerator syntaxGenerator,
        IInvocationOperation invocationOperation)
    {
        var argumentSyntax = GetExceptionCreationExpression(invocationOperation, syntaxGenerator);

        var fromExceptionInvocationExpression =
            syntaxGenerator.InvocationExpression(
                syntaxGenerator.MemberAccessExpression(
                    syntaxGenerator.DottedName("System.Threading.Tasks.Task"), "FromException"),
                argumentSyntax);
        return fromExceptionInvocationExpression;
    }

    private static SyntaxNode GetExceptionCreationExpression(
        IInvocationOperation invocationOperation,
        SyntaxGenerator syntaxGenerator)
    {
        if (invocationOperation.TargetMethod.IsGenericMethod)
        {
            return syntaxGenerator.ObjectCreationExpression(
                syntaxGenerator.TypeExpression(invocationOperation.TargetMethod.TypeArguments.First()));
        }

        if (invocationOperation.Instance != null)
        {
            return invocationOperation.Arguments.First().Value.Syntax;
        }

        return invocationOperation.Arguments.OrderBy(arg => arg.Parameter.Ordinal)
            .First(arg => arg.Parameter.Ordinal > 0).Value.Syntax;
    }
}