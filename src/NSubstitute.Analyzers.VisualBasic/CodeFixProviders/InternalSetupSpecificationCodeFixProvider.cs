using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.Refactorings;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal class InternalSetupSpecificationCodeFixProvider : AbstractInternalSetupSpecificationCodeFixProvider<CompilationUnitSyntax>
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.InternalSetupSpecification);

        protected override Task<Document> AddModifierRefactoring(Document document, SyntaxNode node)
        {
            return Refactorings.AddModifierRefactoring.RefactorAsync(document, node, Accessibility.Protected);
        }

        protected override Task<Document> ReplaceModifierRefactoring(Document document, SyntaxNode node)
        {
            return Refactorings.ReplaceModifierRefactoring.RefactorAsync(document, node, Accessibility.Internal, Accessibility.Public);
        }

        protected override void RegisterAddInternalsVisibleToAttributeCodeFix(CodeFixContext context, Diagnostic diagnostic, CompilationUnitSyntax compilationUnitSyntax)
        {
            AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, diagnostic, compilationUnitSyntax);
        }
    }
}