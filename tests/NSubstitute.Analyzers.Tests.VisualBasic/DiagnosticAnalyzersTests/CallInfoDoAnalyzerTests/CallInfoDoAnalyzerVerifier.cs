using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoDoAnalyzerTests
{
    public abstract class CallInfoDoAnalyzerVerifier : CallInfoDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new CallInfoDoAnalyzer();
        }
    }
}