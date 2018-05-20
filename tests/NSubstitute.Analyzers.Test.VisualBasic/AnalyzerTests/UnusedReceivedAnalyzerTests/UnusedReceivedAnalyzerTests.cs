using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Test.VisualBasic.AnalyzerTests.UnusedReceivedAnalyzerTests
{
    public abstract class UnusedReceivedAnalyzerTests : UnusedReceivedAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new UnusedReceivedAnalyzer();
        }
    }
}