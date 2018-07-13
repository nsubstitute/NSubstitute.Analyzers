using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IReEntrantReturnsSetupDiagnosticVerifier
    {
        Task ReturnsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall);

        Task ReturnsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall);

        Task ReturnsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall);

        Task ReturnsDiagnostic_ForNestedReEntrantCall();

        Task ReturnsDiagnostic_ForSpecificNestedReEntrantCall();

        Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall);

        Task ReturnsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall);

        Task ReturnsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn);
    }
}