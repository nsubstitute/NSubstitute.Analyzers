using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Test.CSharp.DiagnosticAnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupAnalyzerTest : NonVirtualSetupAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }
    }
}