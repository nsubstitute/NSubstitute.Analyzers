using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherDiagnosticVerifier : CSharpDiagnosticVerifier, IArgumentMatcherDiagnosticVerifier
    {
        protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedWithoutSpecifyingCall;

        [CombinatoryTheory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("(int)Arg.Any<int>()")]
        [InlineData("Arg.Any<int>() as int?")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("(int)Arg.Compat.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>() as int?")]
        [InlineData("Arg.Is(1)")]
        [InlineData("(int)Arg.Is(1)")]
        [InlineData("Arg.Is(1) as int?")]
        [InlineData("Arg.Compat.Is(1)")]
        [InlineData("(int)Arg.Compat.Is(1)")]
        [InlineData("Arg.Compat.Is(1) as int?")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg);

        [CombinatoryTheory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("(int)Arg.Any<int>()")]
        [InlineData("Arg.Any<int>() as int?")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("(int)Arg.Compat.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>() as int?")]
        [InlineData("Arg.Is(1)")]
        [InlineData("(int)Arg.Is(1)")]
        [InlineData("Arg.Is(1) as int?")]
        [InlineData("Arg.Compat.Is(1)")]
        [InlineData("(int)Arg.Compat.Is(1)")]
        [InlineData("Arg.Compat.Is(1) as int?")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg);

        [CombinatoryTheory]
        [InlineData("[|Arg.Any<int>()|]")]
        [InlineData("[|Arg.Compat.Any<int>()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        [InlineData("[|Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}