using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface INonSubstitutableMemberReceivedDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method);

        Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty(string method);

        Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty(string method);

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer(string method);

        Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer(string method);

        Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

        Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

        Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

        Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call);
    }
}