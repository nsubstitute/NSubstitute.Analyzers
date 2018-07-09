using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : VisualBasicDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }
    }
}