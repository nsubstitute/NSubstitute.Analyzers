using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.CallInfoAnalyzerTests
{
    [SuppressMessage("ReSharper", "xUnit1013", Justification = "Reviewed")]
    public abstract class CallInfoDiagnosticVerifier : CSharpDiagnosticVerifier, ICallInfoDiagnosticVerifier
    {
        public abstract Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string call, string argAccess);

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

        [Theory]
        [InlineData("decimal", "1", "Could not set value of type int to argument 0 (decimal) because the types are incompatible.")]
        [InlineData("string", "new object()", "Could not set value of type object to argument 0 (string) because the types are incompatible.")]
        [InlineData("int", "'1'", "Could not set value of type char to argument 0 (int) because the types are incompatible.")]
        [InlineData("int", "1M", "Could not set value of type decimal to argument 0 (int) because the types are incompatible.")]
        [InlineData("char", "1", "Could not set value of type int to argument 0 (char) because the types are incompatible.")]
        [InlineData("List<object>", "new List<object>().AsReadOnly()", "Could not set value of type System.Collections.ObjectModel.ReadOnlyCollection<object> to argument 0 (System.Collections.Generic.List<object>) because the types are incompatible.")]
        [InlineData("List<object>", "new object[] { new object() }", "Could not set value of type object[] to argument 0 (System.Collections.Generic.List<object>) because the types are incompatible.")]
        public abstract Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string left, string right, string expectedMessage);

        [Theory]
        [InlineData("object", @"""string""")]
        [InlineData("int", "1")]
        [InlineData("int?", "1")]
        [InlineData("decimal", "1M")]
        [InlineData("double", "1D")]
        [InlineData("IEnumerable<object>", "new List<object>()")]
        [InlineData("IDictionary<string, object>", "new Dictionary<string, object>()")]
        [InlineData("IReadOnlyDictionary <string, object>", "new Dictionary<string, object>()")]
        public abstract Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string left, string right);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new CallInfoAnalyzer();
        }
    }
}