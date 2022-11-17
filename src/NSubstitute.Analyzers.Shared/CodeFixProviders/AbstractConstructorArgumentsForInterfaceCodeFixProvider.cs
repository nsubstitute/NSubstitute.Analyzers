using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using Document = Microsoft.CodeAnalysis.Document;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractConstructorArgumentsForInterfaceCodeFixProvider : CodeFixProvider
{
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);

    public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var codeAction = CodeAction.Create(
            "Remove constructor arguments",
            ct => CreateChangedDocument(context, ct),
            nameof(AbstractConstructorArgumentsForInterfaceCodeFixProvider));
        context.RegisterCodeFix(codeAction, context.Diagnostics);

        return Task.CompletedTask;
    }

    protected abstract SyntaxNode GetInvocationExpressionSyntaxWithEmptyArgumentList(IInvocationOperation invocationOperation);

    protected abstract SyntaxNode GetInvocationExpressionSyntaxWithNullConstructorArgument(IInvocationOperation invocationOperation);

    private async Task<Document> CreateChangedDocument(CodeFixContext context, CancellationToken cancellationToken)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);

        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var invocation = root.FindNode(context.Span, getInnermostNodeForTie: true);
        var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);
        if (semanticModel.GetOperation(invocation) is not IInvocationOperation invocationOperation)
        {
            return context.Document;
        }

        var updatedInvocation = invocationOperation.TargetMethod.IsGenericMethod
            ? GetInvocationExpressionSyntaxWithEmptyArgumentList(invocationOperation)
            : GetInvocationExpressionSyntaxWithNullConstructorArgument(invocationOperation);

        documentEditor.ReplaceNode(invocation, updatedInvocation);

        return documentEditor.GetChangedDocument();
    }
}