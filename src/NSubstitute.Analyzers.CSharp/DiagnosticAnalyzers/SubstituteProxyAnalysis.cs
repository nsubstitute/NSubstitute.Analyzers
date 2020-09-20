using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    internal class SubstituteProxyAnalysis : AbstractSubstituteProxyAnalysis<InvocationExpressionSyntax>
    {
        public static SubstituteProxyAnalysis Instance { get; } = new SubstituteProxyAnalysis();

        private SubstituteProxyAnalysis()
        {
        }
    }
}