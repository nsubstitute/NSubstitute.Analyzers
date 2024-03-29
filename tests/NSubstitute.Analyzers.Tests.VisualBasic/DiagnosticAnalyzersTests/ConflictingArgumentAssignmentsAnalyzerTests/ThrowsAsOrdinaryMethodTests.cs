using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ConflictingArgumentAssignmentsAnalyzerTests;

[CombinatoryData("ExceptionExtensions.Throws", "ExceptionExtensions.ThrowsForAnyArgs")]
public class ThrowsAsOrdinaryMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess)
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
               {previousCallArgAccess}
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(value:= {call}, createException:= Function(callInfo)
               {previousCallArgAccess}
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(createException:= Function(callInfo)
               {previousCallArgAccess}
                Return New Exception()
            End Function, value:= {call}).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, Descriptor);
    }

    public override async Task ReportsNoDiagnostics_WhenSubstituteMethodCannotBeInferred(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim configuredCall = {method}(substitute.Bar(Arg.Any(Of Integer)()), Function(callInfo)
                                                                                               callInfo(0) = 1
                                                                                               Return New Exception()
                                                                                           End Function)
            Dim otherCall = {method}(value:= substitute.Bar(Arg.Any(Of Integer)()), createException:= Function(callInfo)
                                                                                               callInfo(0) = 1
                                                                                               Return New Exception()
                                                                                           End Function)
            Dim yetAnotherCall = {method}(createException:= Function(callInfo)
                                                                        callInfo(0) = 1
                                                                        Return New Exception()
                                                                      End Function, value:= substitute.Bar(Arg.Any(Of Integer)()))
            configuredCall.AndDoes(Function(callInfo)
                                       callInfo(0) = 1
                                   End Function)

            otherCall.AndDoes(Function(callInfo)
                                       callInfo(0) = 1
                                   End Function)

            yetAnotherCall.AndDoes(Function(callInfo)
                                       callInfo(0) = 1
                                   End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices
Imports NSubstitute
Imports NSubstitute.Core
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(Arg.Any(Of Integer)()), Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function,
            Function(callInfo)
                callInfo(0) = 1
            End Function)
            {method}(value:= substitute.Bar(Arg.Any(Of Integer)()), createException:= Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function,
            Function(callInfo)
                callInfo(0) = 1
            End Function)
            {method}(createException:= Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function, value:= substitute.Bar(Arg.Any(Of Integer)())).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function,
            Function(callInfo)
                callInfo(0) = 1
            End Function)
        End Sub
    End Class

    Module MyExtensions
        <Extension()>
        Function AndDoes(ByVal [call] As ConfiguredCall, ByVal firstCall As Action(Of CallInfo), ByVal secondCall As Action(Of CallInfo))
        End Function

    End Module

End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess)
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
                callInfo(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(value:= {call}, createException:= Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(createException:= Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function, value:= {call}).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess)
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
            End Function).AndDoes(Function(callInfo)
                {argAccess}
            End Function)

            {method}(value:= {call}, createException:= Function(callInfo)
                {argAccess}
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                {argAccess}
            End Function)

            {method}(createException:= Function(callInfo)
                {argAccess}
                Return New Exception()
            End Function, value:= {call}).AndDoes(Function(callInfo)
                {argAccess}
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Task(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar(Arg.Any(Of Integer)()), Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)

            {method}(value:= substitute.Bar(Arg.Any(Of Integer)()), createException:= Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)

            {method}(createException:= Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return New Exception()
            End Function, value:= substitute.Bar(Arg.Any(Of Integer)())).AndDoes(Function(callInfo)
                callInfo(0) = 1
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess)
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
            {method}({call}, New Exception()).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(value:= {call}, ex:= New Exception()).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)

            {method}(ex:= New Exception(), value:= {call}).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}