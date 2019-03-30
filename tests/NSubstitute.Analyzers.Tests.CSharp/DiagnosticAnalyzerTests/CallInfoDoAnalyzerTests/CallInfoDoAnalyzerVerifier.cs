using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoDoAnalyzerTests
{
    public abstract class CallInfoDoAnalyzerVerifier : CallInfoDiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new CallInfoDoAnalyzer();
        }
    }
}