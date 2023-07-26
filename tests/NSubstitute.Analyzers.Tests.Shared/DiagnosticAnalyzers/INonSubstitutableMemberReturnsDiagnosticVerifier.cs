using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberReturnsDiagnosticVerifier : INonSubstitutableMemberDiagnosticVerifier
{
    /// <summary>
    /// As for today cases where setup is done indirectly e.g.
    /// <code>
    /// var substitute = NSubstitute.Substitute.For&lt;Foo&gt;();
    /// var returnValue = substitute.Bar();
    /// SubstituteExtensions.ReturnsForAnyArgs&lt;int&gt;(returnValue, 1);
    /// </code>
    /// are not correctly analyzed as they require data flow analysys,
    /// this test makes sure that such cases are ignored and does not produces a false warnings.
    /// </summary>
    Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method);

    Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type);

    Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method);

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
}