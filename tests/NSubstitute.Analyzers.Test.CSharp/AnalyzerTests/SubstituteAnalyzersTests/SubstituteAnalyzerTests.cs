using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.SubstituteAnalyzersTests
{
    public class SubstituteAnalyzerTests : AnalyzerTest
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }
    }
}