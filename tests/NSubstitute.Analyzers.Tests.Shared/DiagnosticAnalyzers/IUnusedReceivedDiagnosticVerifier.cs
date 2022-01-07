using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface IUnusedReceivedDiagnosticVerifier
{
    Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method);

    Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess(string method);

    Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess(string method);

    Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess(string method);

    Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method);

    Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method);
}