using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface INonVirtualSetupReceivedDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate();

        Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty();

        Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty();

        Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer();

        Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer();
    }
}