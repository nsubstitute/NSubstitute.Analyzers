using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    public abstract class CallInfoDiagnosticVerifier : CSharpDiagnosticVerifier, ICallInfoDiagnosticVerifier
    {
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn);

        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string call, string argAccess);

        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess);

        public abstract Task ReportsNoDiagnostic_WhenConvertingTypeToAssignableTypeForIndirectCasts(string call, string argAccess);

        public abstract Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

        public abstract Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string call, string argAccess);

        public abstract Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string call, string argAccess);

        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string call, string argAccess);

        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string call);

        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string call);

        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string call);

        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string call);

        public abstract Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string call);

        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument();

        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument();

        public abstract Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument();

        public abstract Task ReportsDiagnostic_WhenAssigningWrongTypeToArgument();

        public abstract Task ReportsNoDiagnostic_WhenAssigningProperTypeToArgument();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new CallInfoAnalyzer();
        }
    }
}