using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface ISyncOverAsyncThrowsDiagnosticVerifier
{
    Task ReportsDiagnostic_WhenUsedInTaskReturningMethod(string method);

    Task ReportsDiagnostic_WhenUsedInTaskReturningProperty(string method);

    Task ReportsDiagnostic_WhenUsedInTaskReturningIndexer(string method);

    Task ReportsNoDiagnostic_WhenUsedWithSyncMember(string method);
}