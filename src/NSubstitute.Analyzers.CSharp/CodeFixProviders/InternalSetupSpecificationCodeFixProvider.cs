using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.Refactorings;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    public class InternalSetupSpecificationCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.InternalSetupSpecification);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic =
                context.Diagnostics.FirstOrDefault(diag => diag.Id == DiagnosticIdentifiers.InternalSetupSpecification);

            if (diagnostic == null)
            {
                return;
            }

            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var invocationExpression = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var syntaxReference = await GetDeclaringSyntaxReference(context, invocationExpression);

            if (syntaxReference == null)
            {
                return;
            }

            var syntaxNode = await syntaxReference.GetSyntaxAsync();

            if (syntaxNode == null)
            {
                return;
            }

            context.RegisterCodeFix(CodeAction.Create("Add protected modifier", token => AddModifierRefactoring.RefactorAsync(context.Document, syntaxNode, Accessibility.Protected)), diagnostic);

            var compilationUnitSyntax = FindCompilationUnitSyntax(syntaxNode);

            if (compilationUnitSyntax == null)
            {
                return;
            }

            AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, diagnostic, compilationUnitSyntax);
        }

        private async Task<SyntaxReference> GetDeclaringSyntaxReference(CodeFixContext context, SyntaxNode invocationExpression)
        {
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
            var symbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol;

            var firstOrDefault = symbol.DeclaringSyntaxReferences.FirstOrDefault();

            return firstOrDefault;
        }

        private CompilationUnitSyntax FindCompilationUnitSyntax(SyntaxNode syntaxNode)
        {
            return syntaxNode.Parent.Ancestors().OfType<CompilationUnitSyntax>().LastOrDefault();
        }
    }
}