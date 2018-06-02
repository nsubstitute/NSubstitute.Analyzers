using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
#if CSHARP
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
#elif VISUAL_BASIC
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;
#endif
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CodeFixProviders
{
#if CSHARP
    [ExportCodeFixProvider(LanguageNames.CSharp)]
#elif VISUAL_BASIC
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
#endif
    public class ForPartsOfUsedForUnsupportedTypeCodeFixProvider : CodeFixProvider
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

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var forPartsOfNode = (InvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var nameNode = GetGenericNameSyntax(forPartsOfNode);
            var forNode = forPartsOfNode.ReplaceNode(nameNode, nameNode.WithIdentifier(IdentifierName("For").Identifier));

            var replaceNode = root.ReplaceNode(forPartsOfNode, forNode);

            return context.Document.WithSyntaxRoot(replaceNode);
        }

        private static GenericNameSyntax GetGenericNameSyntax(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return (GenericNameSyntax)memberAccess.Name;
        }
    }
}