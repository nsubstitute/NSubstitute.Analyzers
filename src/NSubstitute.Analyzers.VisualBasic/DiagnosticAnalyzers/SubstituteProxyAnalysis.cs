using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    internal sealed class SubstituteProxyAnalysis : AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax>
    {
        public static SubstituteProxyAnalysis Instance { get; } = new SubstituteProxyAnalysis();

        private SubstituteProxyAnalysis()
        {
        }
    }
}