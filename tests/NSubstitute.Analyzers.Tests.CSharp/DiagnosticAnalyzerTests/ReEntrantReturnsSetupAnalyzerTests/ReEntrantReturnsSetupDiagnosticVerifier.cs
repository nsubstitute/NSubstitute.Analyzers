using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }
    }
}