using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface ICallInfoDiagnosticVerifier
    {
        Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string argAccess, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string argAccess);

        Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string argAccess, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string argAccess);

        Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string argAccess);

        Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string argAccess);

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation();

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation();

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation();

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation();

        Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument();

        Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument();

        Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument();

        Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument();

        Task ReportsDiagnostic_WhenAssigningWrongTypeToArgument();

        Task ReportsNoDiagnostic_WhenAssigningProperTypeToArgument();
    }
}