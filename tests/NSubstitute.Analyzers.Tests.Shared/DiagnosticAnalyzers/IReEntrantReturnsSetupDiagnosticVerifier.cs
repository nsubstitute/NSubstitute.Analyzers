using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IReEntrantReturnsSetupDiagnosticVerifier
    {
        Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall);

        Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall);

        Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall);

        Task ReportsDiagnostic_ForNestedReEntrantCall();

        Task ReportsDiagnostic_ForSpecificNestedReEntrantCall();

        Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string localVariable);

        Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall);

        Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall);

        Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn);

        Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles();
    }
}