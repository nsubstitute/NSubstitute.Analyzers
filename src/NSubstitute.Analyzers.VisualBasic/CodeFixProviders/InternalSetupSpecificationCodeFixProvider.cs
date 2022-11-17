using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.Refactorings;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class InternalSetupSpecificationCodeFixProvider : AbstractInternalSetupSpecificationCodeFixProvider<CompilationUnitSyntax>
{
    protected override string ReplaceModifierCodeFixTitle { get; } = "Replace friend with public modifier";

    protected override Task<Document> AddModifierRefactoring(
        Document document,
        SyntaxNode node,
        Accessibility accessibility,
        CancellationToken cancellationToken)
    {
        return Refactorings.AddModifierRefactoring.RefactorAsync(document, node, accessibility, cancellationToken);
    }

    protected override Task<Document> ReplaceModifierRefactoring(
        Document document,
        SyntaxNode node,
        Accessibility fromAccessibility,
        Accessibility toAccessibility,
        CancellationToken cancellationToken)
    {
        return Refactorings.ReplaceModifierRefactoring.RefactorAsync(document, node, fromAccessibility, toAccessibility, cancellationToken);
    }

    protected override void RegisterAddInternalsVisibleToAttributeCodeFix(CodeFixContext context, CompilationUnitSyntax compilationUnitSyntax)
    {
        AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, compilationUnitSyntax);
    }
}