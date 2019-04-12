using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.Refactorings;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal class SubstituteForInternalMemberCodeFixProvider : AbstractSubstituteForInternalMemberCodeFixProvider<InvocationExpressionSyntax, ExpressionSyntax, CompilationUnitSyntax>
    {
        protected override AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax, ExpressionSyntax> GetSubstituteProxyAnalysis()
        {
            return new SubstituteProxyAnalysis();
        }

        protected override void RegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, CompilationUnitSyntax compilationUnitSyntax)
        {
            AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, diagnostic, compilationUnitSyntax);
        }
    }
}