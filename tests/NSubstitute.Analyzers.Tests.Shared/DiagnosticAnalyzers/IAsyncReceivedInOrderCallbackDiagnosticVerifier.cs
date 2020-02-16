using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IAsyncReceivedInOrderCallbackDiagnosticVerifier
    {
        Task ReportsDiagnostic_WhenAsyncLambdaCallbackUsedInReceivedInOrder();

        Task ReportsDiagnostic_WhenAsyncDelegateCallbackUsedInReceivedInOrder();

        Task ReportsNoDiagnostic_WhenNonAsyncLambdaCallbackUsedInReceivedInOrder();

        Task ReportsNoDiagnostic_WhenNonAsyncDelegateCallbackUsedInReceivedInOrder();

        Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod();
    }
}