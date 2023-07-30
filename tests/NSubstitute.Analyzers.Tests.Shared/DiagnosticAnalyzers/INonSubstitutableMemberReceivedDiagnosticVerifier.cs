using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberReceivedDiagnosticVerifier : INonSubstitutableMemberDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsedWithNonVirtualEvent(string method);

    Task ReportsNoDiagnostics_WhenUsedWithAbstractEvent(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualEvent(string method);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceEvent(string method);
}