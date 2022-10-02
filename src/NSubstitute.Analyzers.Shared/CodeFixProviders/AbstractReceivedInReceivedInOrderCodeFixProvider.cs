using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using Document = Microsoft.CodeAnalysis.Document;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractReceivedInReceivedInOrderCodeFixProvider : CodeFixProvider
{
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.ReceivedUsedInReceivedInOrder);

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var codeAction = CodeAction.Create(
            "Remove redundant Received checks",
            ct => CreateChangedDocument(ct, context),
            nameof(AbstractReceivedInReceivedInOrderCodeFixProvider));

        context.RegisterCodeFix(codeAction, context.Diagnostics);

        return Task.CompletedTask;
    }

    private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var invocation = root.FindNode(context.Span, getInnermostNodeForTie: true);

        var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);
        if (semanticModel.GetOperation(invocation) is not IInvocationOperation invocationOperation)
        {
            return context.Document;
        }

        var syntax = invocationOperation.Instance != null
            ? invocationOperation.Instance.Syntax
            : invocationOperation.Arguments.Single(arg => arg.Parameter.Ordinal == 0).Value.Syntax;

        var updatedRoot = root.ReplaceNode(
            invocation,
            syntax.WithTriviaFrom(invocation));

        return context.Document.WithSyntaxRoot(updatedRoot);
    }
}