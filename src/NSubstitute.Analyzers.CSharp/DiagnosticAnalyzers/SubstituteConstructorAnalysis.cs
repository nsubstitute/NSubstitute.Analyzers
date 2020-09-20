using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class SubstituteConstructorAnalysis : AbstractSubstituteConstructorAnalysis<InvocationExpressionSyntax>
    {
        public static SubstituteConstructorAnalysis Instance { get; } = new SubstituteConstructorAnalysis();

        private SubstituteConstructorAnalysis()
        {
        }
    }
}