using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;
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
            if (diagnostic == null)
            {
                return Task.CompletedTask;
            }

            var codeAction = CodeAction.Create(
                "Remove constructor arguments",
                ct => CreateChangedDocument(ct, context, diagnostic),
                nameof(AbstractConstructorArgumentsForInterfaceCodeFixProvider<TInvocationExpressionSyntax>));

            context.RegisterCodeFix(codeAction, diagnostic);

            return Task.CompletedTask;
        }

        protected abstract SyntaxNode GetExpression(TInvocationExpressionSyntax invocationExpressionSyntax);

        private async Task<Document> CreateChangedDocument(CancellationToken cancellationToken, CodeFixContext context, Diagnostic diagnostic)
        {
            var documentEditor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
            var syntaxGenerator = documentEditor.Generator;

            var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var invocation = (TInvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);

            if (!(semanticModel.GetOperation(invocation) is IInvocationOperation invocationOperation))
            {
                return context.Document;
            }

            var expression = GetExpression(invocation);
            var updatedInvocation = invocationOperation.TargetMethod.IsGenericMethod
                ? GetInvocationExpressionSyntaxWithEmptyArgumentList(syntaxGenerator, expression)
                : GetInvocationExpressionSyntaxWithNullConstructorArgument(syntaxGenerator, invocationOperation,  expression);

            documentEditor.ReplaceNode(invocation, updatedInvocation);

            return documentEditor.GetChangedDocument();
        }

        private static SyntaxNode GetInvocationExpressionSyntaxWithEmptyArgumentList(
            SyntaxGenerator syntaxGenerator,
            SyntaxNode expression)
        {
            return syntaxGenerator.InvocationExpression(expression);
        }

        private static SyntaxNode GetInvocationExpressionSyntaxWithNullConstructorArgument(
            SyntaxGenerator generator,
            IInvocationOperation invocationOperation,
            SyntaxNode expression)
        {
            var nullArgument = generator.Argument(generator.NullLiteralExpression());
            var newArguments = new[]
            {
                invocationOperation.GetOrderedArgumentOperationsWithoutInstanceArgument().First().Syntax, nullArgument
            };

            return generator.InvocationExpression(expression, newArguments);
        }
    }
}