using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Document = Microsoft.CodeAnalysis.Document;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal class AbstractConstructorArgumentsForInterfaceCodeFixProvider : CodeFixProvider
    {
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.FirstOrDefault(diag => diag.Descriptor.Id == DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface);
            if (diagnostic != null)
            {
                var codeAction = CodeAction.Create("Use Substitute.For", ct => CreateChangedDocument(ct, context, diagnostic), "equvalencyKey");
                context.RegisterCodeFix(codeAction, diagnostic);
            }

            return Task.FromResult(1);
        }

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            return await Task.FromResult(context.Document);
            /*
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var invocation = (InvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);
            ArgumentListSyntax argumentListSyntax;
            if (semanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol methodSymbol && methodSymbol.IsGenericMethod)
            {
                argumentListSyntax = ArgumentList();
            }
            else
            {
                // TODO consider making the vb and c# analyzers/fixproviders independent
#if CSHARP
                var nullSyntax = Argument(LiteralExpression(SyntaxKind.NullLiteralExpression));
                var seconArgument = invocation.ArgumentList.Arguments.Skip(1).First();
                argumentListSyntax = invocation.ArgumentList.ReplaceNode(seconArgument, nullSyntax);
#elif VISUAL_BASIC
                var nullSyntax = SimpleArgument(LiteralExpression(SyntaxKind.NothingLiteralExpression, Token(SyntaxKind.NothingKeyword)));
                var seconArgument = invocation.ArgumentList.Arguments.Skip(1).First();
                argumentListSyntax = invocation.ArgumentList.ReplaceNode(seconArgument, nullSyntax);
#endif
            }

            var updatedInvocation = invocation.WithArgumentList(argumentListSyntax);
            var replacedRoot = root.ReplaceNode(invocation, updatedInvocation);
            return context.Document.WithSyntaxRoot(replacedRoot);
            */
        }
    }
}