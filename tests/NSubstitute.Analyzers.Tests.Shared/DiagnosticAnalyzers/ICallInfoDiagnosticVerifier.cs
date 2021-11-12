using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers
{
    public interface ICallInfoDiagnosticVerifier
    {
        Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess);

        Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method, string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBoundsForNestedCall(string method);

        Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call, string argAccess);

        Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call, string argAccess);

        Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call, string argAccess);

        Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call, string argAccess, string message);

        Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string method, string call, string argAccess);

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method, string call, string argAccess, string message);

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string method, string call, string argAccess);

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInvocationForNestedCall(string method);

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method, string call, string argAccess, string message);

        Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call, string argAccess);

        Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method, string call);

        Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method);

        Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method);

        Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method);

        Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method, string left, string right, string expectedMessage);

        Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method, string left, string right);

        Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocationForNestedCall(string method);

        Task ReportsDiagnostic_WhenAccessingArgumentOutOfBoundsForNestedCall(string method);
    }
}