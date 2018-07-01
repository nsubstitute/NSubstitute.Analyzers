using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface INonVirtualSetupWhenDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string whenAction, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string whenAction, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string whenAction);

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string whenAction);

        Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string whenAction, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string whenAction);

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string whenAction, int expectedLine, int expectedColumn);

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction();

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction();
    }
}