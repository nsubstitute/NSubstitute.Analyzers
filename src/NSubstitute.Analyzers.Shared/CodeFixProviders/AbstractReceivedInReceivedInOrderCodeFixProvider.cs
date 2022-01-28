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
        var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.ReceivedUsedInReceivedInOrder);
        if (diagnostic != null)
        {
            var codeAction = CodeAction.Create("Remove redundant Received checks", ct => CreateChangedDocument(ct, context, diagnostic), nameof(AbstractReceivedInReceivedInOrderCodeFixProvider));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        return Task.CompletedTask;
    }

    private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
    {
        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var invocation = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

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