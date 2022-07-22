using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SyncOverAsyncThrowsAnalyzerTests;

public class ThrowsAsOrdinaryMethodWithGenericTypeSpecifiedTests : SyncOverAsyncThrowsDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_WhenUsedInTaskReturningMethod(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
        Function FooBar() As Task(Of Exception)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            [|ExceptionExtensions.{method}(Of Exception)(substitute.Bar())|]
            [|ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar())|]
            [|ExceptionExtensions.{method}(Of Exception)(substitute.FooBar())|]
            [|ExceptionExtensions.{method}(Of Exception)(value : =substitute.FooBar())|]
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, SyncOverAsyncThrowsDescriptor);
    }

    public override async Task ReportsDiagnostic_WhenUsedInTaskReturningProperty(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Property Bar As Task
        Property FooBar As Task(Of Object)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            [|ExceptionExtensions.{method}(Of Exception)(substitute.Bar)|]
            [|ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar)|]
            [|ExceptionExtensions.{method}(Of Exception)(substitute.FooBar)|]
            [|ExceptionExtensions.{method}(Of Exception)(value := substitute.FooBar)|]
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, SyncOverAsyncThrowsDescriptor);
    }

    public override async Task ReportsDiagnostic_WhenUsedInTaskReturningIndexer(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer) As Task
        Default Property Item(ByVal x As Integer, ByVal y As Integer) As Task(Of Object)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            [|ExceptionExtensions.{method}(Of Exception)(substitute(0))|]
            [|ExceptionExtensions.{method}(Of Exception)(value := substitute(0))|]
            [|ExceptionExtensions.{method}(Of Exception)(substitute(0, 0))|]
            [|ExceptionExtensions.{method}(Of Exception)(value := substitute(0, 0))|]
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, SyncOverAsyncThrowsDescriptor);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithSyncMember(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Object
        Property FooBar() As Object
        Default Property Item(ByVal x As Integer) As Object
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            ExceptionExtensions.{method}(Of Exception)(substitute.Bar())
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar())
            ExceptionExtensions.{method}(Of Exception)(substitute.FooBar)
            ExceptionExtensions.{method}(Of Exception)(value := substitute.FooBar)
            ExceptionExtensions.{method}(Of Exception)(substitute(0))
            ExceptionExtensions.{method}(Of Exception)(value := substitute(0))
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenThrowsAsyncUsed(string method)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
        Function FooBar() As Task(Of Object)

        Property Foo As Task
        Property FooBarBar As Task(Of Object)

        Default Property Item(ByVal x As Integer) As Task
        Default Property Item(ByVal x As Integer, ByVal y As Integer) As Task(Of Object)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            ExceptionExtensions.{method}(Of Exception)(substitute.Bar())
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar())
            ExceptionExtensions.{method}(Of Exception)(substitute.FooBar())
            ExceptionExtensions.{method}(Of Exception)(value := substitute.FooBar())

            ExceptionExtensions.{method}(Of Exception)(substitute.Foo)
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Foo)
            ExceptionExtensions.{method}(Of Exception)(substitute.FooBarBar)
            ExceptionExtensions.{method}(Of Exception)(value := substitute.FooBarBar)

            ExceptionExtensions.{method}(Of Exception)(substitute(0))
            ExceptionExtensions.{method}(Of Exception)(value := substitute(0))
            ExceptionExtensions.{method}(Of Exception)(substitute(0, 0))
            ExceptionExtensions.{method}(Of Exception)(value := substitute(0, 0))
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}