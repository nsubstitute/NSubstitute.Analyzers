using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests
{
    [SuppressMessage("ReSharper", "xUnit1013", Justification = "Reviewed")]
    public abstract class CallInfoDiagnosticVerifier : VisualBasicDiagnosticVerifier, ICallInfoDiagnosticVerifier
    {
        public abstract Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string call, string argAccess);

        public abstract Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn);

        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string call, string argAccess);

        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess);

        public abstract Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string call, string argAccess);

        public abstract Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

        public abstract Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string call, string argAccess);

        public abstract Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

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

        [Theory]
        [InlineData("Decimal", "1", "Could not set value of type Integer to argument 0 (Decimal) because the types are incompatible.")]
        [InlineData("String", "new object()", "Could not set value of type Object to argument 0 (String) because the types are incompatible.")]
        [InlineData("Integer", @"""1""c", "Could not set value of type Char to argument 0 (Integer) because the types are incompatible.")]
        [InlineData("Integer", "1D", "Could not set value of type Decimal to argument 0 (Integer) because the types are incompatible.")]
        [InlineData("Char", "1", "Could not set value of type Integer to argument 0 (Char) because the types are incompatible.")]
        [InlineData("List(Of Object)", "New List(Of Object)().AsReadOnly()", "Could not set value of type System.Collections.ObjectModel.ReadOnlyCollection(Of Object) to argument 0 (System.Collections.Generic.List(Of Object)) because the types are incompatible.")]
        [InlineData("List(Of Object)", "New Object() {New Object()}", "Could not set value of type Object() to argument 0 (System.Collections.Generic.List(Of Object)) because the types are incompatible.")]
        public abstract Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string left, string right, string expectedMessage);

        [Theory]
        [InlineData("Object", @"""string""")]
        [InlineData("Integer", "1")]
        [InlineData("Integer?", "1")]
        [InlineData("Decimal", "1D")]
        [InlineData("Double", "1R")]
        [InlineData("IEnumerable(Of Object)", "New List(Of Object)()")]
        [InlineData("IDictionary(Of String, Object)", "New Dictionary(Of String, Object)()")]
        [InlineData("IReadOnlyDictionary(Of String, Object)", "New Dictionary(Of String, Object)()")]
        public abstract Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string left, string right);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new CallInfoAnalyzer();
        }
    }
}