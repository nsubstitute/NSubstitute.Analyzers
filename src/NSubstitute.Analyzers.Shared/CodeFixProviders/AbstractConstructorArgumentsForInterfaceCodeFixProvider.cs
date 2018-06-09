using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
#if CSHARP
    [ExportCodeFixProvider(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
#endif
    public class AbstractConstructorArgumentsForInterfaceCodeFixProvider : CodeFixProvider
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

            return Task.CompletedTask;
        }

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
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
        }
    }
}