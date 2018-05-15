using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupAnalyzerTest : NonVirtualSetupAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }
    }
}