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
        [Theory]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo(1) = 1")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.Args()(1) = 1")]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.ArgTypes()(1) = GetType(Integer)")]
        [InlineData("substitute.Barr", "callInfo.ArgAt(Of Integer)(1)")]
        [InlineData("substitute.Barr", "Dim x = callInfo(1)")]
        [InlineData("substitute.Barr", "callInfo(1) = 1")]
        [InlineData("substitute.Barr", "Dim x = callInfo.Args()(1)")]
        [InlineData("substitute.Barr", "callInfo.Args()(1) = 1")]
        [InlineData("substitute.Barr", "callInfo.ArgTypes()(1) = GetType(Integer)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo(1) = 1")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.Args()(1) = 1")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.ArgTypes()(1) = GetType(Integer)")]
        public abstract Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string call, string argAccess);

        [Theory]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)", 15, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo(1)", 15, 40)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo(1) = 1", 15, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)", 15, 40)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.Args()(1) = 1", 15, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.ArgTypes()(1) = GetType(Integer)", 15, 32)]
        [InlineData("substitute.Barr", "callInfo.ArgAt(Of Integer)(1)", 15, 32)]
        [InlineData("substitute.Barr", "Dim x = callInfo(1)", 15, 40)]
        [InlineData("substitute.Barr", "callInfo(1) = 1", 15, 32)]
        [InlineData("substitute.Barr", "Dim x = callInfo.Args()(1)", 15, 40)]
        [InlineData("substitute.Barr", "callInfo.Args()(1) = 1", 15, 32)]
        [InlineData("substitute.Barr", "callInfo.ArgTypes()(1) = GetType(Integer)", 15, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)", 15, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo(1)", 15, 40)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo(1) = 1", 15, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)", 15, 40)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.Args()(1) = 1", 15, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.ArgTypes()(1) = GetType(Integer)", 15, 32)]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                  callInfo.ArgAt(Of Integer)(x)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                 Dim y = callInfo(x)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                  Dim y = callInfo.Args()(x)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                  Dim y = callInfo.ArgTypes()(x)")]
        [InlineData("substitute.Barr", @"Dim x = 2
                                         callInfo.ArgAt(Of Integer)(x)")]
        [InlineData("substitute.Barr", @"Dim x = 2
                                         Dim y = callInfo(x)")]
        [InlineData("substitute.Barr", @"Dim x = 2
                                         Dim y = callInfo.Args()(x)")]
        [InlineData("substitute.Barr", @"Dim x = 2
                                         Dim y = callInfo.ArgTypes()(x)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                      callInfo.ArgAt(Of Integer)(x)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                      Dim y = callInfo(x)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                      Dim y = callInfo.Args()(x)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", @"Dim x = 2
                                                                                      Dim y = callInfo.ArgTypes()(x)")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string call, string argAccess);

        [Theory]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(0)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo(0)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(0)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.ArgTypes()(0)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.ArgTypes()(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(0)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo(0)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(0)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.ArgTypes()(0)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())", "Dim x = callInfo.ArgTypes()(1)")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo(1), BarBase)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo(1), BarBase)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo(1), BarBase)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.Args()(1), BarBase)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo.Args()(1), BarBase)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.Args()(1), BarBase)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo(1), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo(1), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo(1), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.Args()(1), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo.Args()(1), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.Args()(1), Object)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo(1), BarBase)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo(1), BarBase)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo(1), BarBase)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.Args()(1), BarBase)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo.Args()(1), BarBase)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.Args()(1), BarBase)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo(1), Object)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo(1), Object)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo(1), Object)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.Args()(1), Object)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = CType(callInfo.Args()(1), Object)")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.Args()(1), Object)")]
        public abstract Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string call, string argAccess);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "callInfo.ArgAt(Of Bar)(1)", 23, 32)]
        [InlineData("substitute.Foo(Arg.Any(Of Integer)(), Arg.Any(Of FooBar)())", "callInfo.ArgAt(Of Bar)(1)", 23, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo(1), Bar)", 23, 46)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo(1), Bar)", 23, 48)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo(1), Bar)", 23, 51)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo.Args()(1), Bar)", 23, 46)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo.Args()(1), Bar)", 23, 48)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo.Args()(1), Bar)", 23, 51)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "callInfo.ArgAt(Of Bar)(1)", 23, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo(1), Bar)", 23, 46)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo(1), Bar)", 23, 48)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo(1), Bar)", 23, 51)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo.Args()(1), Bar)", 23, 46)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo.Args()(1), Bar)", 23, 48)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo.Args()(1), Bar)", 23, 51)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of FooBar)())", "Dim x = callInfo.ArgAt(Of Bar)(1)", 23, 40)]
        public abstract Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "callInfo.ArgAt(Of Bar)(0)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo(0), Bar)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = CType(callInfo(0), Bar)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo(0), Bar)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.Args()(0), Bar)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = CType(callInfo.Args()(0), Bar)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.Args()(0), Bar)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "callInfo.ArgAt(Of Bar)(0)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo(0), Bar)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = CType(callInfo(0), Bar)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo(0), Bar)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.Args()(0), Bar)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = CType(callInfo.Args()(0), Bar)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.Args()(0), Bar)")]
        public abstract Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string call, string argAccess);

        public abstract Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn, string message);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.ArgTypes(), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.ArgTypes(), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = CType(callInfo.ArgTypes(), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.ArgTypes()(0), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.ArgTypes()(0), Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "Dim x = CType(callInfo.ArgTypes()(0), Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.ArgTypes(), Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.ArgTypes(), Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = CType(callInfo.ArgTypes(), Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = TryCast(callInfo.ArgTypes()(0), Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = DirectCast(callInfo.ArgTypes()(0), Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "Dim x = CType(callInfo.ArgTypes()(0), Object)")]
        public abstract Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string call, string argAccess);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "callInfo.ArgTypes()(0) = GetType(Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "callInfo.Args()(0) = 1D")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "callInfo.ArgTypes()(0) = GetType(Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "callInfo.Args()(0) = 1D")]
        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string call, string argAccess);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())")]
        [InlineData("substitute.Barr")]
        [InlineData("substitute(Arg.Any(Of Integer)())")]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string call);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())")]
        [InlineData("substitute(Arg.Any(Of Integer)())")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string call);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())")]
        public abstract Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string call);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        public abstract Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string call);

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        public abstract Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string call);

        [Fact]
        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument();

        [Fact]
        public abstract Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument();

        [Fact]
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