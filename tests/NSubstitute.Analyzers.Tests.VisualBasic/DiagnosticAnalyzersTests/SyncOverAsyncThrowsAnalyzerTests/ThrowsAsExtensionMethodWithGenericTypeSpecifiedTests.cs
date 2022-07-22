using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SyncOverAsyncThrowsAnalyzerTests;

public class ThrowsAsExtensionMethodWithGenericTypeSpecifiedTests : SyncOverAsyncThrowsDiagnosticVerifier
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
        Function FooBar() As Task(Of Object)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            [|substitute.Bar().{method}(Of Exception)|]
            [|substitute.FooBar().{method}(Of Exception)|]
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
            [|substitute.Bar.{method}(Of Exception)|]
            [|substitute.FooBar.{method}(Of Exception)|]
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
            [|substitute(0).{method}(Of Exception)|]
            [|substitute(0, 0).{method}(Of Exception)|]
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
        Property FooBar As Object
        Default Property Item(ByVal x As Integer) As Object
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(Of Exception)
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
            substitute.Bar().{method}(Of Exception)
            substitute.FooBar().{method}(Of Exception)

            substitute.Foo.{method}(Of Exception)
            substitute.FooBarBar.{method}(Of Exception)

            substitute(0).{method}(Of Exception)
            substitute(0, 0).{method}(Of Exception)
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}