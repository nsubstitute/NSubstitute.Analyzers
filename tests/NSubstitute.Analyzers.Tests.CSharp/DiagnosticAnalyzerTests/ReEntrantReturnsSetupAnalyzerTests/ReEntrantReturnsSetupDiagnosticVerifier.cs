using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : CSharpDiagnosticVerifier, IReEntrantReturnsSetupDiagnosticVerifier
    {
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall);

        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall);

        public abstract Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall);

        public abstract Task ReportsDiagnostic_ForNestedReEntrantCall();

        public abstract Task ReportsDiagnostic_ForSpecificNestedReEntrantCall();

        public abstract Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string localVariable);

        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall);

        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall);

        public abstract Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn);

        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }
    }
}