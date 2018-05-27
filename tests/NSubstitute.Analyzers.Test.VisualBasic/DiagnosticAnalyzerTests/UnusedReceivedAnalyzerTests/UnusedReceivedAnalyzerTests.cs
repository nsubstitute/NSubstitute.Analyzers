using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;

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