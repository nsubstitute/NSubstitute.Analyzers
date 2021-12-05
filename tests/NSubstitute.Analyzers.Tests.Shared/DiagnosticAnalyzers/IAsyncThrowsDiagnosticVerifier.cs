using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface IAsyncThrowsDiagnosticVerifier
    {
        Task ReportsDiagnostic_WhenUsedWithNonGenericAsyncMethod(string method);

        Task ReportsDiagnostic_WhenUsedWithGenericAsyncMethod(string method);

        Task ReportsNoDiagnostic_WhenUsedWithSyncMethod(string method);
    }
}