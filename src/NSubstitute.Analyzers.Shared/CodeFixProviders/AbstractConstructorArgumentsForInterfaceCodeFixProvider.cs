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
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.FirstOrDefault(diag =>
            diag.Descriptor.Id == DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);
        if (diagnostic == null)
        {
            return Task.CompletedTask;
        }

        var codeAction = CodeAction.Create(
            "Remove constructor arguments",
            ct => CreateChangedDocument(ct, context, diagnostic),
            nameof(AbstractConstructorArgumentsForInterfaceCodeFixProvider));
        context.RegisterCodeFix(codeAction, diagnostic);

        return Task.CompletedTask;
    }

    protected abstract SyntaxNode GetInvocationExpressionSyntaxWithEmptyArgumentList(IInvocationOperation invocationOperation);

    protected abstract SyntaxNode GetInvocationExpressionSyntaxWithNullConstructorArgument(IInvocationOperation invocationOperation);

    private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);

        var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

        var invocation = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
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