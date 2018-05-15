using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Test.VisualBasic.AnalyzerTests.NonVirtualSetupAnalyzerTests
{
    public abstract class NonVirtualSetupAnalyzerTest : NonVirtualSetupAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }
    }
}