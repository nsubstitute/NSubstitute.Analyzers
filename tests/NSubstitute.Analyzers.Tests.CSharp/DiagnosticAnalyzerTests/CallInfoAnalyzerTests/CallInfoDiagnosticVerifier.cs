using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    public class CallInfoDiagnosticVerifier : CSharpDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new CallInfoAnalyzer();
        }
    }
}