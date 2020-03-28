using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<TInvocationExpression, TNameSyntax>
        : CodeFixProvider
        where TInvocationExpression : SyntaxNode
        where TNameSyntax : SyntaxNode
    {
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.PartialSubstituteForUnsupportedType);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.PartialSubstituteForUnsupportedType);
            if (diagnostic == null)
            {
                return;
            }

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var invocationExpression = (TInvocationExpression)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);

            if (!(semanticModel.GetSymbolInfo(invocationExpression).Symbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            var title = methodSymbol.Name == MetadataNames.SubstituteFactoryCreatePartial
                ? "Use SubstituteFactory.Create"
                : "Use Substitute.For";
            var codeAction = CodeAction.Create(
                title,
                ct => CreateChangedDocument(context, methodSymbol, invocationExpression),
                nameof(AbstractPartialSubstituteUsedForUnsupportedTypeCodeFixProvider<TInvocationExpression, TNameSyntax>));

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        protected abstract TNameSyntax GetNameSyntax(TInvocationExpression methodInvocationNode);

        private async Task<Document> CreateChangedDocument(CodeFixContext context, IMethodSymbol methodSymbol, TInvocationExpression invocationExpression)
        {
            var nameNode = GetNameSyntax(invocationExpression);
            var documentEditor = await DocumentEditor.CreateAsync(context.Document);
            var generator = documentEditor.Generator;

            var updateNameNode = methodSymbol.IsGenericMethod
                ? generator.GenericName(MetadataNames.NSubstituteForMethod, methodSymbol.TypeArguments)
                : generator.IdentifierName(MetadataNames.SubstituteFactoryCreate);

            var updatedInvocationExpression = invocationExpression.ReplaceNode(nameNode, updateNameNode);

            documentEditor.ReplaceNode(invocationExpression, updatedInvocationExpression);

            return documentEditor.GetChangedDocument();
        }
    }
}