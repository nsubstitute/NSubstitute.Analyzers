using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal abstract class AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider : CodeFixProvider
{
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(DiagnosticIdentifiers.PartialSubstituteForUnsupportedType);

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var syntaxNode = root.FindNode(context.Span, getInnermostNodeForTie: true);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

        if (semanticModel.GetOperation(syntaxNode) is not IInvocationOperation invocationOperation)
        {
            return;
        }

        var title = invocationOperation.TargetMethod.Name == MetadataNames.SubstituteFactoryCreatePartial
            ? "Use SubstituteFactory.Create"
            : "Use Substitute.For";

        var codeAction = CodeAction.Create(
            title,
            ct => CreateChangedDocument(context, invocationOperation, ct),
            nameof(AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider));
        context.RegisterCodeFix(codeAction, context.Diagnostics);
    }

    protected abstract SyntaxNode UpdateInvocationExpression(
        IInvocationOperation invocationOperation,
        string identifierName);

    private async Task<Document> CreateChangedDocument(
        CodeFixContext context,
        IInvocationOperation invocationOperation,
        CancellationToken cancellationToken)
    {
        var documentEditor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
        var newIdentifierName = invocationOperation.TargetMethod.IsGenericMethod
            ? MetadataNames.NSubstituteForMethod
            : MetadataNames.SubstituteFactoryCreate;

        var updatedInvocationExpression = UpdateInvocationExpression(invocationOperation, newIdentifierName);

        documentEditor.ReplaceNode(invocationOperation.Syntax, updatedInvocationExpression);

        return documentEditor.GetChangedDocument();
    }
}