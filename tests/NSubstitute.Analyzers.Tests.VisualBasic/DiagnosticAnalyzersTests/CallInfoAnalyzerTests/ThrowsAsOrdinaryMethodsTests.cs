using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests
{
    public class ThrowsAsOrdinaryMethodsTests : CallInfoDiagnosticVerifier
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
        public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default  ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnedValue = {call}
            ExceptionExtensions.Throws(returnedValue, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)", 16, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo(1)", 16, 40)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo(1) = 1", 16, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)", 16, 40)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.Args()(1) = 1", 16, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)())", "callInfo.ArgTypes()(1) = GetType(Integer)", 16, 32)]
        [InlineData("substitute.Barr", "callInfo.ArgAt(Of Integer)(1)", 16, 32)]
        [InlineData("substitute.Barr", "Dim x = callInfo(1)", 16, 40)]
        [InlineData("substitute.Barr", "callInfo(1) = 1", 16, 32)]
        [InlineData("substitute.Barr", "Dim x = callInfo.Args()(1)", 16, 40)]
        [InlineData("substitute.Barr", "callInfo.Args()(1) = 1", 16, 32)]
        [InlineData("substitute.Barr", "callInfo.ArgTypes()(1) = GetType(Integer)", 16, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.ArgAt(Of Integer)(1)", 16, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo(1)", 16, 40)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo(1) = 1", 16, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "Dim x = callInfo.Args()(1)", 16, 40)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.Args()(1) = 1", 16, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())", "callInfo.ArgTypes()(1) = GetType(Integer)", 16, 32)]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string call, string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
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
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

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
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

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
        public override async Task ReportsNoDiagnostic_WhenConvertingTypeToAssignableTypeForIndirectCasts(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Bar) As Integer
    End Interface

    Public Class BarBase
    End Class

    Public Class Bar
        Inherits BarBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "callInfo.ArgAt(Of Bar)(1)", 24, 32)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo(1), Bar)", 24, 46)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo(1), Bar)", 24, 48)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo(1), Bar)", 24, 51)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo.Args()(1), Bar)", 24, 46)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo.Args()(1), Bar)", 24, 48)]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo.Args()(1), Bar)", 24, 51)]
        [InlineData("substitute.Foo(Arg.Any(Of Integer)(), Arg.Any(Of FooBar)())", "callInfo.ArgAt(Of Bar)(1)", 24, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "callInfo.ArgAt(Of Bar)(1)", 24, 32)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo(1), Bar)", 24, 46)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo(1), Bar)", 24, 48)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo(1), Bar)", 24, 51)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = CType(callInfo.Args()(1), Bar)", 24, 46)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = TryCast(callInfo.Args()(1), Bar)", 24, 48)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())", "Dim x = DirectCast(callInfo.Args()(1), Bar)", 24, 51)]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of FooBar)())", "Dim x = callInfo.ArgAt(Of Bar)(1)", 24, 40)]
        public override async Task ReportsDiagnostic_WhenConvertingTypeToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Function Foo(ByVal x As Integer, ByVal bar As FooBar) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal bar As FooBar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooBar
        Inherits Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
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
        public override async Task ReportsNoDiagnostic_WhenConvertingTypeToSupportedType(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

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
        public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "callInfo.ArgTypes()(0) = GetType(Object)")]
        [InlineData("substitute.Bar(Arg.Any(Of Bar)())", "callInfo.Args()(0) = 1D")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "callInfo.ArgTypes()(0) = GetType(Object)")]
        [InlineData("substitute(Arg.Any(Of Bar)())", "callInfo.Args()(0) = 1D")]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string call, string argAccess)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Integer
        Default ReadOnly Property Item(ByVal x As Bar) As Integer
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())")]
        [InlineData("substitute.Barr")]
        [InlineData("substitute(Arg.Any(Of Integer)())")]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string call)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer

    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               callInfo.Arg(Of Double)()
                               Return New Exception()
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
                    new DiagnosticResultLocation(17, 32)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)())")]
        [InlineData("substitute(Arg.Any(Of Integer)())")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string call)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               callInfo.Arg(Of Integer)()
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Integer)())")]
        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string call)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               callInfo.Arg(Of Integer)()
                               Return New Exception()
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
                    new DiagnosticResultLocation(15, 32)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string call)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               callInfo.Arg(Of Integer)()
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("substitute.Bar(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        [InlineData("substitute(Arg.Any(Of Integer)(), Arg.Any(Of Double)())")]
        public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string call)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws({call}, Function(callInfo)
                               callInfo(1) = 1
                               Return New Exception()
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
                    new DiagnosticResultLocation(15, 32)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
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
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
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
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws(substitute.Bar(value), Function(callInfo)
                                              callInfo(1) = 1
                                              Return New Exception()
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
                    new DiagnosticResultLocation(16, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenAssigningWrongTypeToArgument()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Decimal) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Decimal = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
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
                    new DiagnosticResultLocation(16, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public override async Task ReportsNoDiagnostic_WhenAssigningProperTypeToArgument()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Decimal) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Decimal = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            ExceptionExtensions.Throws(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = 1D
                                              Return New Exception()
                                          End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }
    }
}