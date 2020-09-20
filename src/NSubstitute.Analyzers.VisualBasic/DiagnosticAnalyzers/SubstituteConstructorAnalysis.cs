using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal class SubstituteConstructorAnalysis : AbstractSubstituteConstructorAnalysis<InvocationExpressionSyntax>
    {
        public static SubstituteConstructorAnalysis Instance { get; } = new SubstituteConstructorAnalysis();

        private SubstituteConstructorAnalysis()
        {
        }
    }
}