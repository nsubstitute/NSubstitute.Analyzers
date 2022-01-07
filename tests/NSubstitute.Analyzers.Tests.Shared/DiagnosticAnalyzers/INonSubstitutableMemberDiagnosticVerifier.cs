using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method);

    Task ReportsDiagnostics_WhenSettingValueForLiteral(string method, string literal, string type);

    Task ReportsDiagnostics_WhenSettingValueForStaticMethod(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method);

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
    Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method);

    Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method);

    Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method);

    Task ReportsNoDiagnostics_WhenSettingValueForVirtualIndexer(string method);

    Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method);

    Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method);

    Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call);
}