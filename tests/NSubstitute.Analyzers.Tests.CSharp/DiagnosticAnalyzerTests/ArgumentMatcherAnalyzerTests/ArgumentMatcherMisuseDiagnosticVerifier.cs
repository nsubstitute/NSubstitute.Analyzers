using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherMisuseDiagnosticVerifier : CSharpDiagnosticVerifier, IArgumentMatcherMisuseDiagnosticVerifier
    {
        protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedWithoutSpecifyingCall;

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

        [CombinatoryTheory]
        [CombinatoryData("Arg.Do<int>(1)", "Arg.Compat.Do<int>(1)", "Arg.Invoke<int>(1)", "Arg.Compat.Invoke<int>(1)", "Arg.InvokeDelegate<int>(1)", "Arg.Compat.InvokeDelegate<int>(1)")]
        [InlineData("[|NSubstitute.Arg.Any<int>()|]")]
        [InlineData("[|NSubstitute.Arg.Compat.Any<int>()|]")]
        [InlineData("[|NSubstitute.Arg.Is(1)|]")]
        [InlineData("[|NSubstitute.Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUseTogetherWithUnfortunatelyNamedArgDoInvoke(string argDoInvoke, string arg);

        [CombinatoryTheory]
        [CombinatoryData("Arg.Do<int>(_ => {})", "Arg.Compat.Do<int>(_ => {})")]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForMethodCall(string argDo, string arg);

        [CombinatoryTheory]
        [CombinatoryData("Arg.Do<int>(_ => {})", "Arg.Compat.Do<int>(_ => {})")]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForIndexerCall(string argDo, string arg);

        [CombinatoryTheory]
        [CombinatoryData("Arg.Invoke<int>(1)", "Arg.Compat.Invoke<int>(1)", "Arg.InvokeDelegate<Action<int>>(1)", "Arg.Compat.InvokeDelegate<Action<int>>(1)")]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForMethodCall(string argInvoke, string arg);

        [CombinatoryTheory]
        [CombinatoryData("Arg.Invoke<int>(1)", "Arg.Compat.Invoke<int>(1)", "Arg.InvokeDelegate<Action<int>>(1)", "Arg.Compat.InvokeDelegate<Action<int>>(1)")]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Compat.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForIndexerCall(string argInvoke, string arg);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}