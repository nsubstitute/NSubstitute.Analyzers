using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ConflictingArgumentAssignmentsAnalyzerTests
{
    public abstract class ConflictingArgumentAssignmentsDiagnosticVerifier : VisualBasicDiagnosticVerifier, IConflictingArgumentAssignmentsDiagnosticVerifier
    {
        public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ConflictingArgumentAssignments;

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo(1) = 1", "[|callInfo(1)|] = 1")]
        [InlineData("substitute.Barr", "callInfo(1) = 1", "[|callInfo(1)|] = 1")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo(1) = 1", "[|callInfo(1)|] = 1")]
        public abstract Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo(1) = 1")]
        [InlineData("substitute.Barr", "callInfo(1) = 1")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo(1) = 1")]
        public abstract Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo(1)")]
        [InlineData("substitute.Barr", "Dim x = callInfo(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo(1)")]
        public abstract Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo(1) = 1")]
        [InlineData("substitute.Barr", "callInfo(1) = 1")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo(1) = 1")]
        public abstract Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ConflictingArgumentAssignmentsAnalyzer();
        }
    }
}