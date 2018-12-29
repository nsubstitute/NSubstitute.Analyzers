using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests
{
    public class ThrowsAsExtensionMethodsTests : CallInfoDiagnosticVerifier
    {
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
            returnedValue.Throws(Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

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
            {call}.Throws(Function(callInfo)
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
            {call}.Throws(Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace";
            await VerifyDiagnostic(source);
        }

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
            {call}.Throws(Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string call, string argAccess)
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
            {call}.Throws(Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn)
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
            {call}.Throws(Function(callInfo)
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

        public override async Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string call, string argAccess)
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
            {call}.Throws(Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string call, string argAccess, int expectedLine, int expectedColumn, string message)
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
            {call}.Throws(Function(callInfo)
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
                Message = message,
                Locations = new[]
                {
                    new DiagnosticResultLocation(expectedLine, expectedColumn)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

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
            {call}.Throws(Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

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
            {call}.Throws(Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string call, string argAccess, string message)
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
            {call}.Throws(Function(callInfo)
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

        public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string call,
            string argAccess)
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
            {call}.Throws(Function(callInfo)
                               callInfo.Arg(Of Integer)()
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string call,
            string argAccess, string message)
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
            {call}.Throws(Function(callInfo)
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
            {call}.Throws(Function(callInfo)
                               callInfo.Arg(Of Integer)()
                               Return New Exception()
                           End Function)
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source);
        }

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
            {call}.Throws(Function(callInfo)
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
            substitute.Bar(value).Throws(Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

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
            substitute.Bar(value).Throws(Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function)
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

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
            substitute.Bar(value).Throws(Function(callInfo)
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

        public override async Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string left, string right, string expectedMessage)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As {left}) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As {left} = Nothing
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Throws(Function(callInfo)
                                              callInfo(0) = {right}
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
                Message = expectedMessage,
                Locations = new[]
                {
                    new DiagnosticResultLocation(17, 47)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string left, string right)
        {
            var source = $@"
Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As {left}) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As {left} = Nothing
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Bar(value).Throws(Function(callInfo)
                                              callInfo(0) = {right}
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