using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method);

    Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type);

    Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method);

    Task ReportsNoDiagnostics_WhenUsedWithForNonSealedOverrideMethod(string method);

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

    Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);
}