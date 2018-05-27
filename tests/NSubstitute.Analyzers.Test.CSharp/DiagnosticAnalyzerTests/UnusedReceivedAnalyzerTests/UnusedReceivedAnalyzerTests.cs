using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Test.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public abstract class UnusedReceivedAnalyzerTests : UnusedReceivedAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new UnusedReceivedAnalyzer();
        }
    }
}