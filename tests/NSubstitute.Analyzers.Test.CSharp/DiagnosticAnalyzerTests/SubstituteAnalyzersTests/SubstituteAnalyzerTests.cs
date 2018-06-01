using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Test.CSharp.DiagnosticAnalyzerTests.SubstituteAnalyzersTests
{
    public abstract class SubstituteAnalyzerTests : SubstituteAnalyzerTestBase
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }
    }
}