using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ConflictingArgumentAssignmentsAnalyzerTests
{
    public abstract class ConflictingArgumentAssignmentsDiagnosticVerifier : CSharpDiagnosticVerifier, IConflictingArgumentAssignmentsDiagnosticVerifier
    {
        public DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ConflictingAssignmentsToOutRefArgument;

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;", "[|callInfo[1]|] = 1;")]
        public abstract Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;")]
        public abstract Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo[1];")]
        [InlineData("substitute.Barr", "var x = callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo[1];")]
        public abstract Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method);

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;")]
        public abstract Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ConflictingArgumentAssignmentsAnalyzer();
        }
    }
}