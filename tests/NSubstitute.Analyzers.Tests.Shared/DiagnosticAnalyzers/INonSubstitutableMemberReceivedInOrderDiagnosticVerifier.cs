using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface INonSubstitutableMemberReceivedInOrderDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenInvokingNonVirtualMethodWithoutAssignment();

        Task ReportsDiagnostics_WhenInvokingNonVirtualMethodWithNonUsedAssignment();

        Task ReportsDiagnostics_WhenInvokingNonVirtualPropertyWithoutAssignment();

        Task ReportsDiagnostics_WhenInvokingNonVirtualPropertyWithNonUsedAssignment();

        Task ReportsDiagnostics_WhenInvokingNonVirtualIndexerWithoutAssignment();

        Task ReportsDiagnostics_WhenInvokingNonVirtualIndexerWithNonUsedAssignment();

        Task ReportsNoDiagnostics_WhenInvokingNonVirtualMethodWithUsedAssignment();

        Task ReportsNoDiagnostics_WhenInvokingNonVirtualPropertyWithUsedAssignment();

        Task ReportsNoDiagnostics_WhenInvokingNonVirtualIndexerWithUsedAssignment();

        Task ReportsNoDiagnostics_WhenNonVirtualMethodIsCalledAsArgument();

        Task ReportsNoDiagnostics_WhenNonVirtualPropertyIsCalledAsArgument();

        Task ReportsNoDiagnostics_WhenNonVirtualIndexerIsCalledAsArgument();

        Task ReportsNoDiagnostics_WhenInvokingProtectedInternalVirtualMember();

        Task ReportsNoDiagnostics_WhenInvokingVirtualMember();

        Task ReportsDiagnostics_WhenInvokingInternalVirtualMember_AndInternalsVisibleToNotApplied();

        Task ReportsNoDiagnostics_WhenInvokingInternalVirtualMember_AndInternalsVisibleToApplied();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod();
    }
}