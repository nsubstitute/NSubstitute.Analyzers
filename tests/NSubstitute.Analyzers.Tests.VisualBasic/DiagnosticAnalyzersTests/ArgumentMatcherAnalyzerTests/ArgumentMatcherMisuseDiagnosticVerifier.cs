using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherMisuseDiagnosticVerifier : VisualBasicDiagnosticVerifier, IArgumentMatcherMisuseDiagnosticVerifier
    {
        protected DiagnosticDescriptor ArgumentMatcherUsedOutsideOfCallDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedOutsideOfCall;

        [Theory]
        [InlineData("[|Arg.Any(Of Integer)()|]")]
        [InlineData("[|Arg.Compat.Any(Of Integer)()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        [InlineData("[|Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForMethodCall(string arg);

        [Theory]
        [InlineData("[|Arg.Any(Of Integer)()|]")]
        [InlineData("[|Arg.Compat.Any(Of Integer)()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        [InlineData("[|Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUsedWithoutSubstituteMethod_ForIndexerCall(string arg);

        [Theory]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedArgMethod(string arg);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}