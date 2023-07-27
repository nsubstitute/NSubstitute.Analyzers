using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberWhenDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method, string whenAction);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualMemberFromBaseClass(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method, string whenAction);

    Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method, string whenAction);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method, string whenAction);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method, string whenAction);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularFunction(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularFunction(string method);

    Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);
}