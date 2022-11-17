using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.CSharp.Refactorings;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
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