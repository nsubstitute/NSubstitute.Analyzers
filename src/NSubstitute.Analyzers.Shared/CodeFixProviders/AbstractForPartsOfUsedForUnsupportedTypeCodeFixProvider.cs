using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractForPartsOfUsedForUnsupportedTypeCodeFixProvider<TInvocationExpression, TGenericNameSyntax, TIdentifierNameSyntax, TNameSyntax>
        : CodeFixProvider
        where TInvocationExpression : SyntaxNode
        where TGenericNameSyntax : TNameSyntax
        where TIdentifierNameSyntax : TNameSyntax
        where TNameSyntax : SyntaxNode
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

            return Task.CompletedTask;
        }

        protected abstract TInnerNameSyntax GetNameSyntax<TInnerNameSyntax>(TInvocationExpression methodInvocationNode) where TInnerNameSyntax : TNameSyntax;

        protected abstract TInnerNameSyntax GetUpdatedNameSyntax<TInnerNameSyntax>(TInnerNameSyntax nameSyntax, string identifierName) where TInnerNameSyntax : TNameSyntax;

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var forPartsOfNode = (TInvocationExpression)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);

            SyntaxNode nameNode;
            SyntaxNode updateNameNode;

            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);
            var symbolInfo = semanticModel.GetSymbolInfo(forPartsOfNode);

            if (symbolInfo.Symbol is IMethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
            {
                var genericNameSyntax = GetNameSyntax<TGenericNameSyntax>(forPartsOfNode);
                nameNode = genericNameSyntax;
                updateNameNode = GetUpdatedNameSyntax<TGenericNameSyntax>(genericNameSyntax, MetadataNames.NSubstituteForMethod);
            }
            else
            {
                var identifierNameSyntax = GetNameSyntax<TIdentifierNameSyntax>(forPartsOfNode);
                nameNode = identifierNameSyntax;
                updateNameNode = GetUpdatedNameSyntax<TIdentifierNameSyntax>(identifierNameSyntax, MetadataNames.SubstituteFactoryCreate);
            }

            var forNode = forPartsOfNode.ReplaceNode(nameNode, updateNameNode);

            var replaceNode = root.ReplaceNode(forPartsOfNode, forNode);

            return context.Document.WithSyntaxRoot(replaceNode);
        }
    }
}