using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface INonVirtualSetupWhenDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method, string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method, string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method, string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction(string method);

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction(string method);
    }
}