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
    internal sealed class SubstituteForInternalMemberCodeFixProvider : AbstractSubstituteForInternalMemberCodeFixProvider<InvocationExpressionSyntax, ExpressionSyntax, CompilationUnitSyntax>
    {
        public SubstituteForInternalMemberCodeFixProvider()
            : base(SubstituteProxyAnalysis.Instance)
        {
        }

        protected override void RegisterCodeFix(CodeFixContext context, Diagnostic diagnostic, CompilationUnitSyntax compilationUnitSyntax)
        {
            AddInternalsVisibleToAttributeRefactoring.RegisterCodeFix(context, diagnostic, compilationUnitSyntax);
        }
    }
}