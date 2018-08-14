using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface ICallInfoDiagnosticVerifier
    {
        Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess);

        Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string call, string argAccess);

        Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string call, string argAccess);

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string call);

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string call);

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string call);

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string call);

        Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string call);

        Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument();

        Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument();

        Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument();

        Task ReportsDiagnostic_WhenAssigningWrongTypeToArgument();

        Task ReportsNoDiagnostic_WhenAssigningProperTypeToArgument();
    }
}