using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ConflictingArgumentAssignmentsAnalyzerTests;

[CombinatoryData("Throws", "ThrowsAsync", "ThrowsForAnyArgs", "ThrowsAsyncForAnyArgs")]
public class ThrowsAsExtensionMethodTests : ConflictingArgumentAssignmentsDiagnosticVerifier
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
            {call}.{method}(Function(callInfo)
               {previousCallArgAccess}
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
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
            Dim configuredCall = substitute.Bar(Arg.Any(Of Integer)()).{method}(Function(callInfo)
                                                                                               callInfo(0) = 1
                                                                                               Return New Exception()
                                                                                           End Function)
            configuredCall.AndDoes(Function(callInfo)
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
            substitute.Bar(Arg.Any(Of Integer)()).{method}(Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
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
            {call}.{method}(Function(callInfo)
                callInfo(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
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
            {call}.{method}(Function(callInfo)
                {argAccess}
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
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
            substitute.Bar(Arg.Any(Of Integer)()).{method}(Function(callInfo)
                callInfo.Args()(0) = 1
                callInfo.ArgTypes()(0) = GetType(Integer)
                Dim x = (DirectCast(callInfo(0), Byte()))
                x(0) = 1
                Return New Exception()
            End Function).AndDoes(Function(callInfo)
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
            {call}.{method}(New Exception()).AndDoes(Function(callInfo)
                {andDoesArgAccess}
            End Function)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}