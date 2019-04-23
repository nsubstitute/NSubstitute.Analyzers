using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherMisuseDiagnosticVerifier : CSharpDiagnosticVerifier, IArgumentMatcherMisuseDiagnosticVerifier
    {
        protected DiagnosticDescriptor ArgumentMatcherUsedOutsideOfCallDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedOutsideOfCall;

        [Theory]
        [InlineData("[|Arg.Any<int>()|]")]
        [InlineData("[|Arg.Compat.Any<int>()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        [InlineData("[|Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForMethodCall(string arg);

        [Theory]
        [InlineData("[|Arg.Any<int>()|]")]
        [InlineData("[|Arg.Compat.Any<int>()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        [InlineData("[|Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForIndexerCall(string arg);

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedArgMethod(string arg);
        
        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}