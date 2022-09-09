using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;

public interface INonSubstitutableMemberArgumentMatcherDiagnosticVerifier
{
    Task ReportsDiagnostics_WhenUsedInNonVirtualMethod(string arg);

    Task ReportsDiagnostics_WhenUsedInStaticMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInVirtualMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInNonSealedOverrideMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInDelegate(string arg);

    Task ReportsDiagnostics_WhenUsedInSealedOverrideMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInAbstractMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInInterfaceMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInGenericInterfaceMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedInInterfaceIndexer(string arg);

    Task ReportsNoDiagnostics_WhenUsedInVirtualIndexer(string arg);

    Task ReportsDiagnostics_WhenUsedInNonVirtualIndexer(string arg);

    Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string arg);

    Task ReportsNoDiagnostics_WhenUsedWithPotentiallyValidAssignment(string arg);

    Task ReportsDiagnostics_WhenUsedAsStandaloneExpression(string arg);

    Task ReportsDiagnostics_WhenUsedInConstructor(string arg);

    Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToNotApplied(string arg);

    Task ReportsNoDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToApplied(string arg);

    Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string arg);

    Task ReportsNoDiagnostics_WhenUsedInProtectedInternalVirtualMember(string arg);

    Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string arg);

    Task ReportsNoDiagnostics_WhenSubscribingToEvent();

    Task ReportsNoDiagnostics_WhenAssigningAllowedArgMatchersToSubstitutableMember(string arg);

    Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember(string arg);

    Task ReportsDiagnostics_WhenDirectlyAssigningNotAllowedArgMatchersToMember(string arg);

    Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethod(string receivedMethod, string arg);

    Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg);

    Task ReportsNoDiagnostics_WhenAssigningArgMatchersToSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg);
}