using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.Threading;
using Document = Microsoft.CodeAnalysis.Document;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal abstract class AbstractConstructorArgumentsForInterfaceCodeFixProvider<TInvocationExpressionSyntax>
        : CodeFixProvider
    where TInvocationExpressionSyntax : SyntaxNode
    {
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);
            if (diagnostic != null)
            {
                var codeAction = CodeAction.Create("Remove constructor arguments", ct => CreateChangedDocument(ct, context, diagnostic), "equvalencyKey");
                context.RegisterCodeFix(codeAction, diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        protected abstract TInvocationExpressionSyntax GetInvocationExpressionSyntaxWithEmptyArgumentList(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract TInvocationExpressionSyntax GetInvocationExpressionSyntaxWithNullConstructorArgument(TInvocationExpressionSyntax invocationExpressionSyntax);

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var invocation = (TInvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);
            var updatedInvocation = semanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol methodSymbol &&
                                    methodSymbol.IsGenericMethod
                ? GetInvocationExpressionSyntaxWithEmptyArgumentList(invocation)
                : GetInvocationExpressionSyntaxWithNullConstructorArgument(invocation);

            var replacedRoot = root.ReplaceNode(invocation, updatedInvocation);
            return context.Document.WithSyntaxRoot(replacedRoot);
        }
    }
}