using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherMisuseDiagnosticVerifier : VisualBasicDiagnosticVerifier, IArgumentMatcherMisuseDiagnosticVerifier
    {
        protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedWithoutSpecifyingCall;

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

        [CombinatoryTheory]
        [CombinatoryData("Arg.Do(Of Integer)(1)", "Arg.Compat.Do(Of Integer)(1)", "Arg.Invoke(Of Integer)(1)", "Arg.Compat.Invoke(Of Integer)(1)", "Arg.InvokeDelegate(Of Integer)(1)", "Arg.Compat.InvokeDelegate(Of Integer)(1)")]
        [InlineData("[|NSubstitute.Arg.Any(Of Integer)()|]")]
        [InlineData("[|NSubstitute.Arg.Compat.Any(Of Integer)()|]")]
        [InlineData("[|NSubstitute.Arg.Is(1)|]")]
        [InlineData("[|NSubstitute.Arg.Compat.Is(1)|]")]
        public abstract Task ReportsDiagnostics_WhenUseTogetherWithUnfortunatelyNamedArgDoInvoke(string argDoInvoke, string arg);

        [CombinatoryTheory]
        [CombinatoryData(
            @"Arg.Do(Of Integer)(Function()
        End Function)",
            @"Arg.Compat.Do(Of Integer)(Function()
        End Function)")]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForMethodCall(string argDo, string arg);

        [CombinatoryTheory]
        [CombinatoryData(
            @"Arg.Do(Of Integer)(Function()
        End Function)",
            @"Arg.Compat.Do(Of Integer)(Function()
        End Function)")]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgDo_ForIndexerCall(string argDo, string arg);

        [CombinatoryTheory]
        [CombinatoryData("Arg.Invoke(Of Integer)(1)", "Arg.Compat.Invoke(Of Integer)(1)", "Arg.InvokeDelegate(Of Action(Of Integer))(1)", "Arg.Compat.InvokeDelegate(Of Action(Of Integer))(1)")]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForMethodCall(string argInvoke, string arg);

        [CombinatoryTheory]
        [CombinatoryData("Arg.Invoke(Of Integer)(1)", "Arg.Compat.Invoke(Of Integer)(1)", "Arg.InvokeDelegate(Of Action(Of Integer))(1)", "Arg.Compat.InvokeDelegate(Of Action(Of Integer))(1)")]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUseTogetherWithArgInvoke_ForIndexerCall(string argInvoke, string arg);

        [Theory]
        [InlineData("Arg.Any(Of Integer)()")]
        [InlineData("Arg.Compat.Any(Of Integer)()")]
        [InlineData("Arg.Is(1)")]
        [InlineData("Arg.Compat.Is(1)")]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithPotentiallyValidAssignment(string arg);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}