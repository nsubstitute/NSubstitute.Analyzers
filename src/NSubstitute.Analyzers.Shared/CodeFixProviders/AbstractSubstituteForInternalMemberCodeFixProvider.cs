using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractSubstituteForInternalMemberCodeFixProvider<TInvocationExpressionSyntax, TExpressionSyntax, TCompilationUnitSyntax> : AbstractSuppressDiagnosticsCodeFixProvider
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
        where TCompilationUnitSyntax : SyntaxNode
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.SubstituteForInternalMember);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SubstituteForInternalMember);
            if (diagnostic == null)
            {
                return;
            }

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var findNode = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            if (!(findNode is TInvocationExpressionSyntax invocationExpression))
            {
                return;
            }

            var syntaxReference = await GetDeclaringSyntaxReference(context, invocationExpression);

            if (syntaxReference == null)
            {
                return;
            }

            var syntaxNode = await syntaxReference.GetSyntaxAsync();
            var document = context.Document.Project.Solution.GetDocument(syntaxNode.SyntaxTree);
            var compilationUnitSyntax = FindCompilationUnitSyntax(syntaxNode);

            if (compilationUnitSyntax == null)
            {
                return;
            }

            var codeAction = CodeAction.Create("my name", token => CreateChangedDocument(token, compilationUnitSyntax, document), nameof(AbstractSubstituteForInternalMemberCodeFixProvider<TInvocationExpressionSyntax, TExpressionSyntax, TCompilationUnitSyntax>));
            context.RegisterCodeFix(codeAction, diagnostic);
        }

        protected abstract AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> GetSubstituteProxyAnalysis();

        protected abstract TCompilationUnitSyntax AppendInternalsVisibleToAttribute(TCompilationUnitSyntax compilationUnitSyntax);

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, TCompilationUnitSyntax compilationUnitSyntax, Document document)
        {
            var updatedCompilationUnitSyntax = AppendInternalsVisibleToAttribute(compilationUnitSyntax);
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var replaceNode = root.ReplaceNode(compilationUnitSyntax, updatedCompilationUnitSyntax);
            return document.WithSyntaxRoot(replaceNode);
        }

        private async Task<SyntaxReference> GetDeclaringSyntaxReference(CodeFixContext context, TInvocationExpressionSyntax invocationExpression)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            var proxyAnalysis = GetSubstituteProxyAnalysis();
            var actualProxyTypeSymbol = proxyAnalysis.GetActualProxyTypeSymbol(semanticModel, invocationExpression, methodSymbol);
            var syntaxReference = actualProxyTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault();
            return syntaxReference;
        }

        private TCompilationUnitSyntax FindCompilationUnitSyntax(SyntaxNode syntaxNode)
        {
            return syntaxNode.Parent.Ancestors().OfType<TCompilationUnitSyntax>().LastOrDefault();
        }
    }
}