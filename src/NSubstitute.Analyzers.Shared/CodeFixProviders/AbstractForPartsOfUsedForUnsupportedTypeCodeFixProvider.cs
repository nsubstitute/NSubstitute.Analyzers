using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal class AbstractForPartsOfUsedForUnsupportedTypeCodeFixProvider : CodeFixProvider
    {
        // no completed task in .net standard
        private static Task _completedTask = Task.FromResult(1);

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

            return _completedTask;
        }

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            /*
            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var forPartsOfNode = (InvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var nameNode = GetGenericNameSyntax(forPartsOfNode);
            var forNode = forPartsOfNode.ReplaceNode(nameNode, nameNode.WithIdentifier(IdentifierName("For").Identifier));

            var replaceNode = root.ReplaceNode(forPartsOfNode, forNode);

            return context.Document.WithSyntaxRoot(replaceNode);
            */
            return await Task.FromResult(context.Document);
        }

        /*
        private static GenericNameSyntax GetGenericNameSyntax(InvocationExpressionSyntax methodInvocationNode)
        {
            var memberAccess = (MemberAccessExpressionSyntax)methodInvocationNode.Expression;
            return (GenericNameSyntax)memberAccess.Name;
        }
        */
    }
}