using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IReEntrantReturnsSetupDiagnosticVerifier
    {
        Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string method, string reEntrantCall);

        Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string method, string reEntrantCall);

        Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string method, string reEntrantCall);

        Task ReportsDiagnostic_ForNestedReEntrantCall(string method);

        Task ReportsDiagnostic_ForSpecificNestedReEntrantCall(string method);

        Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string method, string localVariable);

        Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string method, string rootCall, string reEntrantCall);

        Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string method, string rootCall, string reEntrantCall);

        Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string method, string firstReturn, string secondReturn);

        Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles(string method);

        Task ReportsNoDiagnostic_WhenUsed_WithTypeofExpression(string method, string type);

        Task ReportsNoDiagnostics_WhenReturnsValueIsSet_InForEachLoop(string method);

        Task ReportsNoDiagnostics_WhenElementUsedTwice_InForEachLoop(string method);
    }
}