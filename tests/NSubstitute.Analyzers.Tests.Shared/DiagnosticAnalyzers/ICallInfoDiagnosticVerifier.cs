using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface ICallInfoDiagnosticVerifier
    {
        Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string call, string argAccess);

        Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess);

        Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string call, string argAccess);

        Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

        Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string call, string argAccess);

        Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

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

        Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string left, string right, string expectedMessage);

        Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string left, string right);
    }
}