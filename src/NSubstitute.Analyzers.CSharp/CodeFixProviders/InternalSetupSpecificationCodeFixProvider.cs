using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.Refactorings;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class InternalSetupSpecificationCodeFixProvider : AbstractInternalSetupSpecificationCodeFixProvider<CompilationUnitSyntax>
    {
        protected override string ReplaceModifierCodeFixTitle { get; } = "Replace internal with public modifier";

        protected override Task<Document> AddModifierRefactoring(Document document, SyntaxNode node, Accessibility accessibility)
        {
            return Refactorings.AddModifierRefactoring.RefactorAsync(document, node, accessibility);
        }

        protected override Task<Document> ReplaceModifierRefactoring(Document document, SyntaxNode node, Accessibility fromAccessibility, Accessibility toAccessibility)
        {
            return Refactorings.ReplaceModifierRefactoring.RefactorAsync(document, node, fromAccessibility, toAccessibility);
        }

        protected override void RegisterAddInternalsVisibleToAttributeCodeFix(CodeFixContext context, Diagnostic diagnostic, CompilationUnitSyntax compilationUnitSyntax)
        {
            AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, diagnostic, compilationUnitSyntax);
        }
    }
}