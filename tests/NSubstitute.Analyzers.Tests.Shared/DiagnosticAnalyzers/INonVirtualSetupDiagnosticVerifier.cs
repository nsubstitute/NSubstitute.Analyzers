using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface INonVirtualSetupDiagnosticVerifier
    {
        Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod();

        Task ReportsDiagnostics_WhenSettingValueForLiteral(string literal, string type);

        Task ReportsDiagnostics_WhenSettingValueForStaticMethod();

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod();

        Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod();

        /// <summary>
        /// As for today cases where setup is done indirectly e.g
        /// <code>
        /// var substitute = NSubstitute.Substitute.For&lt;Foo&gt;();
        /// var returnValue = substitute.Bar();
        /// SubstituteExtensions.ReturnsForAnyArgs&lt;int&gt;(returnValue, 1);
        /// </code>
        /// are not correctly analyzed as they require data flow analysys,
        /// this test makes sure that such cases are ignored and does not produces a false warnings
        /// </summary>
        /// <returns>Task</returns>
        Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired();

        Task ReportsNoDiagnostics_WhenSettingValueForDelegate();

        Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod();

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod();

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty();

        Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod();

        Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod();

        Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer();

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty();

        Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty();

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty();

        Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer();

        Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer();

        Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace();

        Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod();
    }
}