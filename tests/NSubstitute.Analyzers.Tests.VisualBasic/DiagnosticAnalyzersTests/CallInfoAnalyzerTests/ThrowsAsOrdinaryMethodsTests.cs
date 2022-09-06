using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.CallInfoAnalyzerTests;

[CombinatoryData(
    "ExceptionExtensions.Throws",
    "ExceptionExtensions.ThrowsAsync",
    "ExceptionExtensions.ThrowsForAnyArgs",
    "ExceptionExtensions.ThrowsAsyncForAnyArgs")]
public class ThrowsAsOrdinaryMethodsTests : CallInfoDiagnosticVerifier
{
    public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
        ReadOnly Property Barr As Task(Of Integer)
        Default  ReadOnly Property Item(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim returnedValue = {call}
            {method}(returnedValue, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= returnedValue, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= returnedValue)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBounds(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
        ReadOnly Property Barr As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentOutOfBound_AndPositionIsNotLiteralExpression(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Task(Of Integer)
        ReadOnly Property Barr As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBounds(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal Optional y As Integer = 1) As Task(Of Integer)
        ReadOnly Property Barr As Integer
        Default ReadOnly Property Item(ByVal x As Integer, ByVal Optional y As Integer = 1) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentWithinBoundsForNestedCall(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As Integer, ByVal y As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            {method}(substitute.FooBaz(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()), Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any (Of Integer)()), Function(innerCallInfo)
                    Dim x = outerCallInfo.ArgAt(Of Integer)(1)
                    Dim y = outerCallInfo(1)
                    Dim xx = innerCallInfo.ArgAt(Of Integer)(0)
                    Dim yy = innerCallInfo(0)
                    Return New Exception()
                End Function)
                Return New Exception() 
            End Function)
            {method}(value:= substitute.FooBaz(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()), createException:= Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any (Of Integer)()), Function(innerCallInfo)
                    Dim x = outerCallInfo.ArgAt(Of Integer)(1)
                    Dim y = outerCallInfo(1)
                    Dim xx = innerCallInfo.ArgAt(Of Integer)(0)
                    Dim yy = innerCallInfo(0)
                    Return New Exception()
                End Function)
                Return New Exception() 
            End Function)
            {method}(createException:= Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any (Of Integer)()), Function(innerCallInfo)
                    Dim x = outerCallInfo.ArgAt(Of Integer)(1)
                    Dim y = outerCallInfo(1)
                    Dim xx = innerCallInfo.ArgAt(Of Integer)(0)
                    Dim yy = innerCallInfo(0)
                    Return New Exception()
                End Function)
                Return New Exception() 
            End Function, value:= substitute.FooBaz(Arg.Any(Of Integer)(), Arg.Any(Of Integer)()))
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentOutOfBoundsForNestedCall(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As Integer, ByVal y As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For] (Of IFooBar)()
            {method}(substitute.FooBaz(Arg.Any (Of Integer)(), Arg.Any (Of Integer)()), Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For] (Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any (Of Integer)()), Function(innerCallInfo)
                    Dim x = [|outerCallInfo.ArgAt (Of Integer)(2)|]
                    Dim y = [|outerCallInfo(2)|]
                    Dim z = outerCallInfo(1)
                    Dim xx = [|innerCallInfo.ArgAt (Of Integer)(1)|]
                    Dim yy = [|innerCallInfo(1)|]
                    Dim zz = innerCallInfo(0)
                    Return New Exception()
                End Function)
                Return New Exception()
            End Function)
            {method}(value:= substitute.FooBaz(Arg.Any (Of Integer)(), Arg.Any (Of Integer)()), createException:= Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For] (Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any (Of Integer)()), Function(innerCallInfo)
                    Dim x = [|outerCallInfo.ArgAt (Of Integer)(2)|]
                    Dim y = [|outerCallInfo(2)|]
                    Dim z = outerCallInfo(1)
                    Dim xx = [|innerCallInfo.ArgAt (Of Integer)(1)|]
                    Dim yy = [|innerCallInfo(1)|]
                    Dim zz = innerCallInfo(0)
                    Return New Exception()
                End Function)
                Return New Exception()
            End Function)
            {method}(createException:= Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For] (Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any (Of Integer)()), Function(innerCallInfo)
                    Dim x = [|outerCallInfo.ArgAt (Of Integer)(2)|]
                    Dim y = [|outerCallInfo(2)|]
                    Dim z = outerCallInfo(1)
                    Dim xx = [|innerCallInfo.ArgAt (Of Integer)(1)|]
                    Dim yy = [|innerCallInfo(1)|]
                    Dim zz = innerCallInfo(0)
                    Return New Exception()
                End Function)
                Return New Exception()
            End Function, value:= substitute.FooBaz(Arg.Any (Of Integer)(), Arg.Any (Of Integer)()))
        End Sub
    End Class
End Namespace";
        var textParserResult = TextParser.GetSpans(source);

        var diagnosticMessages = new[]
        {
            "There is no argument at position 2",
            "There is no argument at position 2",
            "There is no argument at position 1",
            "There is no argument at position 1"
        }.Repeat(3).ToArray();

        var diagnostics = textParserResult.Spans.Select((span, idx) => CreateDiagnostic(CallInfoArgumentOutOfRangeDescriptor.OverrideMessage(diagnosticMessages[idx]), span)).ToArray();
        await VerifyDiagnostic(textParserResult.Text, diagnostics);
    }

    public override async Task ReportsNoDiagnostic_WhenManuallyCasting_ToSupportedType(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Bar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Bar) As Task(Of Integer)
    End Interface

    Public Class BarBase
    End Class

    Public Class Bar
        Inherits BarBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                                   {argAccess}
                                   Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenManuallyCasting_ToUnsupportedType(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Function Foo(ByVal x As Integer, ByVal bar As FooBar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal bar As FooBar) As Task(Of Integer)
    End Interface

    Public Class Bar
    End Class

    Public Class FooBar
        Inherits Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, "Couldn't convert parameter at position 1 to type MyNamespace.Bar.");
    }

    public override async Task ReportsNoDiagnostic_WhenCasting_WithArgAt_ToSupportedType(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Bar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Bar) As Task(Of Integer)
    End Interface

    Public Class BarBase
    End Class

    Public Class Bar
        Inherits BarBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenCasting_WithArgAt_ToUnsupportedType(string method, string call, string argAccess, string message)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Function Foo(ByVal x As Integer, ByVal bar As FooBar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal bar As FooBar) As Task(Of Integer)
    End Interface

    Public Class Bar
    End Class

    Public Class FooBar
        Inherits Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoCouldNotConvertParameterAtPositionDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenCastingElementsFromArgTypes(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Bar) As Task(Of Integer)
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToNotRefNorOutArgumentViaIndirectCall(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Bar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Bar) As Task(Of Integer)
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocation(string method, string call, string argAccess, string message)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
        ReadOnly Property Barr As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer) As Task(Of Integer)

    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInInvocation(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
        Function Bar(ByVal x As Foo) As Task(Of Integer)
        Function Bar(ByVal x As Integer, ByVal y As Object) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Foo) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y as Object) As Task(Of Integer)
    End Interface

    Public Class FooBase
    End Class

    Public Class Foo
        Inherits FooBase
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeInInvocationForNestedCall(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As String) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            {method}(substitute.FooBaz(Arg.Any (Of String)()), Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any(Of Integer)()), Function(innerCallInfo)
                    Dim x = outerCallInfo.Arg(Of String)()
                    Dim y = innerCallInfo.Arg(Of Integer)()
                    Return New Exception()
                End Function)
                Return New Exception()
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeMultipleTimesInInvocation(string method, string call, string argAccess, string message)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Integer) As Task(Of Integer)
        Function Bar(ByVal x As Object, ByVal y As Object) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Object, ByVal y As Object) As Task(Of Integer)
    End Interface

    Public Class FooBar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, CallInfoMoreThanOneArgumentOfTypeDescriptor, message);
    }

    public override async Task ReportsNoDiagnostic_WhenAccessingArgumentByTypeMultipleDifferentTypesInInvocation(string method, string call, string argAccess)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Function Bar(ByVal x As Object, ByVal y As FooBar) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Object, ByVal y As FooBar) As Task(Of Integer)
    End Interface

    Public Class FooBar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               {argAccess}
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAssigningValueToNotOutNorRefArgument(string method, string call)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
        Default ReadOnly Property Item(ByVal x As Integer, ByVal y As Double) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}({call}, Function(callInfo)
                               [|callInfo(1)|] = 1
                               Return New Exception()
                           End Function)
            {method}(value:= {call}, createException:= Function(callInfo)
                               [|callInfo(1)|] = 1
                               Return New Exception()
                           End Function)
            {method}(createException:= Function(callInfo)
                               [|callInfo(1)|] = 1
                               Return New Exception()
                           End Function, value:= {call})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoArgumentIsNotOutOrRefDescriptor, "Could not set argument 1 (Double) as it is not an out or ref argument.");
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToRefArgument(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByRef x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function)
            {method}(value:= substitute.Bar(value), createException:= Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function)
            {method}(createException:= Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function, value:= substitute.Bar(value))
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningValueToOutArgument(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function)
            {method}(value:= substitute.Bar(value), createException:= Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function)
            {method}(createException:= Function(callInfo)
                                              callInfo(0) = 1
                                              Return New Exception()
                                          End Function, value:= substitute.Bar(value))
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAssigningValueToOutOfBoundsArgument(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As Integer = 0
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(value), Function(callInfo)
                                              [|callInfo(1)|] = 1
                                              Return New Exception()
                                          End Function)
            {method}(value:= substitute.Bar(value), createException:= Function(callInfo)
                                              [|callInfo(1)|] = 1
                                              Return New Exception()
                                          End Function)
            {method}(createException:= Function(callInfo)
                                              [|callInfo(1)|] = 1
                                              Return New Exception()
                                          End Function, value:= substitute.Bar(value))
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, CallInfoArgumentOutOfRangeDescriptor, "There is no argument at position 1");
    }

    public override async Task ReportsDiagnostic_WhenAssigningType_NotAssignableTo_Argument(string method, string left, string right, string expectedMessage)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As {left}) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As {left} = Nothing
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(value), Function(callInfo)
                                              [|callInfo(0)|] = {right}
                                              Return New Exception()
                                          End Function)
            {method}(value:= substitute.Bar(value), createException:= Function(callInfo)
                                              [|callInfo(0)|] = {right}
                                              Return New Exception()
                                          End Function)
            {method}(createException:= Function(callInfo)
                                              [|callInfo(0)|] = {right}
                                              Return New Exception()
                                          End Function, value:= substitute.Bar(value))
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, CallInfoArgumentSetWithIncompatibleValueDescriptor, expectedMessage);
    }

    public override async Task ReportsNoDiagnostic_WhenAssigningType_AssignableTo_Argument(string method, string left, string right)
    {
        var source = $@"
Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Namespace MyNamespace
    Interface Foo
        Function Bar(<Out> ByRef x As {left}) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim value As {left} = Nothing
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(value), Function(callInfo)
                                              callInfo(0) = {right}
                                              Return New Exception()
                                          End Function)
            {method}(value:= substitute.Bar(value), createException:= Function(callInfo)
                                              callInfo(0) = {right}
                                              Return New Exception()
                                          End Function)
            {method}(createException:= Function(callInfo)
                                              callInfo(0) = {right}
                                              Return New Exception()
                                          End Function, value:= substitute.Bar(value))
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsDiagnostic_WhenAccessingArgumentByTypeNotInInvocationForNestedCall(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Interface IFooBar
        Function FooBaz(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFooBar)()
            {method}(substitute.FooBaz(Arg.Any(Of Integer)()), Function(outerCallInfo)
                Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
                {method}(otherSubstitute.Bar(Arg.Any(Of Integer)()), Function(innerCallInfo)
                    Dim x = [|outerCallInfo.Arg(Of String)()|]
                    Dim y = [|innerCallInfo.Arg(Of String)()|]
                    Return New Exception()
                End Function)
                Return New Exception()
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, CallInfoCouldNotFindArgumentToThisCallDescriptor, "Can not find an argument of type String to this call.");
    }
}