using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberReceivedDiagnosticVerifier : INonSubstitutableMemberDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualEvent(string method);

    Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractEvent(string method);

    Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualEvent(string method);

    Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceEvent(string method);
}