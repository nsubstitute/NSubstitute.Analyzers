using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberWhenDiagnosticVerifier : INonSubstitutableMemberDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsedWithNonVirtualMemberFromBaseClass(string method);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularFunction(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularFunction(string method);
}