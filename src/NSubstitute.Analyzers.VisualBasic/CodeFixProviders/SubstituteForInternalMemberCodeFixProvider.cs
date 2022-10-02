using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Refactorings;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class SubstituteForInternalMemberCodeFixProvider : AbstractSubstituteForInternalMemberCodeFixProvider<CompilationUnitSyntax>
{
    public SubstituteForInternalMemberCodeFixProvider()
        : base(SubstituteProxyAnalysis.Instance)
    {
    }

    protected override void RegisterCodeFix(CodeFixContext context, CompilationUnitSyntax compilationUnitSyntax)
    {
        AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, compilationUnitSyntax);
    }
}