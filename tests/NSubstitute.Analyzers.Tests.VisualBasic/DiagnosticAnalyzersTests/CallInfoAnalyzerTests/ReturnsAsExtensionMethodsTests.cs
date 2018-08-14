using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests
{
    public class ReturnsAsExtensionMethodsTests : CallInfoDiagnosticVerifier
    {
        [Theory]
        [InlineData("", "callInfo.ArgAt(Of Integer)(1)", 13, 63)]
        [InlineData("", "Dim x = callInfo(1)", 13, 71)]
        [InlineData("", "callInfo(1) = 1", 13, 63)]
        [InlineData("", "Dim x = callInfo.Args()(1)", 13, 71)]
        [InlineData("", "callInfo.Args()(1) = 1", 13, 63)]
        [InlineData("", "callInfo.ArgTypes()(1) = GetType(Integer)", 13, 63)]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {call}.Returns(Function(callInfo)
                                                              {argAccess}
                                                              Return 1
                                                          End Function)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentOutOfRange,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is no argument at position 1",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("callInfo.ArgAt(Of Integer)(0)")]
        [InlineData("callInfo.ArgAt(Of Integer)(1)")]
        [InlineData("Dim x = callInfo(0)")]
        [InlineData("Dim x = callInfo(1)")]
        [InlineData("Dim x = callInfo.Args()(0)")]
        [InlineData("Dim x = callInfo.Args()(1)")]
        [InlineData("Dim x = callInfo.ArgTypes()(0)")]
        [InlineData("Dim x = callInfo.ArgTypes()(1)")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()).Returns(Function(callInfo)
                                                                                     {argAccess}
                                                                                     Return 1
                                                                                 End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("callInfo.ArgAt(Of Bar)(1)", 16, 85)]
        [InlineData("Dim x = CType(callInfo(1), Bar)", 16, 99)]
        [InlineData("Dim x = TryCast(callInfo(1), Bar)", 16, 101)]
        [InlineData("Dim x = DirectCast(callInfo(1), Bar)", 16, 104)]
        [InlineData("Dim x = CType(callInfo.Args()(1), Bar)", 16, 99)]
        [InlineData("Dim x = TryCast(callInfo.Args()(1), Bar)", 16, 101)]
        [InlineData("Dim x = DirectCast(callInfo.Args()(1), Bar)", 16, 104)]
        public override async Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)()).Returns(Function(callInfo)
                                                                                    {argAccess}
                                                                                    Return 1
                                                                                End Function)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoCouldNotConvertParameterAtPosition,
                Severity = DiagnosticSeverity.Warning,
                Message = "Couldn't convert parameter at position 1 to type MyNamespace.Bar.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("callInfo.ArgAt(Of Bar)(0)")]
        [InlineData("Dim x = TryCast(callInfo.Args()(0), Bar)")]
        [InlineData("Dim x = DirectCast(callInfo.Args()(0), Bar)")]
        [InlineData("Dim x = CType(callInfo.Args()(0), Bar)")]
        [InlineData("Dim x = TryCast(callInfo(0), Bar)")]
        [InlineData("Dim x = DirectCast(callInfo(0), Bar)")]
        [InlineData("Dim x = CType(callInfo(0), Bar)")]
        public override async Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Bar)()).Returns(Function(callInfo)
                                                          {argAccess}
                                                          Return 1
                                                      End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Dim x = TryCast(callInfo.ArgTypes(), Object)")]
        [InlineData("Dim x = DirectCast(callInfo.ArgTypes(), Object)")]
        [InlineData("Dim x = CType(callInfo.ArgTypes(), Object)")]
        public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Bar)()).Returns(Function(callInfo)
                                                          {argAccess}
                                                          Return 1
                                                      End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("callInfo.ArgTypes()(0) = GetType(Object)")]
        [InlineData("callInfo.Args()(0) = 1D")]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Bar)()).Returns(Function(callInfo)
                                                          {argAccess}
                                                          Return 1
                                                      End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation()
        {
            var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)()).Returns(Function(callInfo)
                                                              callInfo.Arg(Of Double)()
                                                              Return 1
                                                          End Function)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoCouldNotFindArgumentToThisCall,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not find an argument of type Double to this call.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(13, 63)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation()
        {
            var source = @"Imports System
Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)()).Returns(Function(callInfo)
                                                              callInfo.Arg(Of Integer)()
                                                              Return 1
                                                          End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()).Returns(Function(callInfo)
                                                                                     callInfo.Arg(Of Integer)()
                                                                                     Return 1
                                                                                 End Function)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoMoreThanOneArgumentOfType,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is more than one argument of type Integer to this call.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 86)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)()).Returns(Function(callInfo)
                                                                                    callInfo.Arg(Of Integer)()
                                                                                    Return 1
                                                                                End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)()).Returns(Function(callInfo)
                                                                                    callInfo(1) = 1
                                                                                    Return 1
                                                                                End Function)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentIsNotOutOrRef,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not set argument 1 (Double) as it is not an out or ref argument.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 85)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Function Bar(ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Returns(Function(callInfo)
                                              callInfo(0) = 1
                                              Return 1
                                          End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Returns(Function(callInfo)
                                              callInfo(0) = 1
                                              Return 1
                                          End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Returns(Function(callInfo)
                                              callInfo(1) = 1
                                              Return 1
                                          End Function)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentOutOfRange,
                Severity = DiagnosticSeverity.Warning,
                Message = "There is no argument at position 1",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningWrongTypeToArgument()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Decimal) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Decimal = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Returns(Function(callInfo)
                                              callInfo(0) = 1
                                              Return 1
                                          End Function)
        End Sub
    End Class
End Namespace
";

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.CallInfoArgumentSetWithIncompatibleValue,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not set value of type Integer to argument 0 (Decimal) because the types are incompatible.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(14, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningProperTypeToArgument()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Decimal) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Decimal = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Returns(Function(callInfo)
                                              callInfo(0) = 1D
                                              Return 1
                                          End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }
    }
}