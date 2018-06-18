using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractForPartsOfUsedForUnsupportedTypeCodeFixProvider<TInvocationExpression, TGenericNameSyntax>
        : CodeFixProvider
        where TInvocationExpression : SyntaxNode
        where TGenericNameSyntax : SyntaxNode
    {
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface);
            if (diagnostic != null)
            {
                var codeAction = CodeAction.Create("Use Substitute.For", ct => CreateChangedDocument(ct, context, diagnostic), "equvalencyKey");
                context.RegisterCodeFix(codeAction, diagnostic);
            }

            return Task.FromResult(1);
        }

        protected abstract TGenericNameSyntax GetGenericNameSyntax(TInvocationExpression methodInvocationNode);

        protected abstract TGenericNameSyntax GetUpdatedGenericNameSyntax(TGenericNameSyntax nameSyntax, string identifierName);

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var forPartsOfNode = (TInvocationExpression)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var nameNode = GetGenericNameSyntax(forPartsOfNode);
            var updateNameNode = GetUpdatedGenericNameSyntax(nameNode, MetadataNames.NSubstituteForMethod);
            var forNode = forPartsOfNode.ReplaceNode(nameNode, updateNameNode);

            var replaceNode = root.ReplaceNode(forPartsOfNode, forNode);

            return context.Document.WithSyntaxRoot(replaceNode);
        }
    }
}