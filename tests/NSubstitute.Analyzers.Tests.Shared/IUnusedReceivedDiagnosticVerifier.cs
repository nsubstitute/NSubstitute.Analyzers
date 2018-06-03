using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared
{
    public interface IUnusedReceivedDiagnosticVerifier
    {
        Task ReportDiagnostics_WhenUsedWithoutMemberCall();

        Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess();

        Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess();

        Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess();

        Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate();

        Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod();
    }
}