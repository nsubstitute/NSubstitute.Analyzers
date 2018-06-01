using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Test.VisualBasic.DiagnosticAnalyzerTests.SubstituteAnalyzersTests
{
    public abstract class SubstituteAnalyzerTests : SubstituteAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }
    }
}
