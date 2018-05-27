using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;

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