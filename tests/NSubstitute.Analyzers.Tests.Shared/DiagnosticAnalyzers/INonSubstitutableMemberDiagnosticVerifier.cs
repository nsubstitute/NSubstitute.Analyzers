using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method);

    Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method);

    Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method);

    Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method);

    Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method);

    Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);

    Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);
}