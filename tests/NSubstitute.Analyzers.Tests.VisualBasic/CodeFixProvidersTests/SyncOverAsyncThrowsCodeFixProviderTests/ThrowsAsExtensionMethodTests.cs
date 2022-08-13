using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class ThrowsAsExtensionMethodTests : SyncOverAsyncThrowsCodeFixVerifier
{
    public override async Task ReplacesThrowsWithReturns_WhenUsedInMethod(string method, string updatedMethod)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(New Exception())
            substitute.Bar().{method}(ex := New Exception())
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{updatedMethod}(Task.FromException(New Exception()))
            substitute.Bar().{updatedMethod}(Task.FromException(New Exception()))
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource, null, NSubstituteVersion.NSubstitute4_2_2);
    }

    public override async Task ReplacesThrowsWithReturns_WhenUsedInProperty(string method, string updatedMethod)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Property Bar As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar.{method}(New Exception())
            substitute.Bar.{method}(ex := New Exception())
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Property Bar As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar.{updatedMethod}(Task.FromException(New Exception()))
            substitute.Bar.{updatedMethod}(Task.FromException(New Exception()))
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource, null, NSubstituteVersion.NSubstitute4_2_2);
    }

    public override async Task ReplacesThrowsWithReturns_WhenUsedInIndexer(string method, string updatedMethod)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute(0).{method}(New Exception())
            substitute(0).{method}(ex := New Exception())
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute(0).{updatedMethod}(Task.FromException(New Exception()))
            substitute(0).{updatedMethod}(Task.FromException(New Exception()))
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource, null, NSubstituteVersion.NSubstitute4_2_2);
    }

    public override async Task ReplacesThrowsWithThrowsAsync_WhenUsedInMethod(string method, string updatedMethod)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(New Exception())
            substitute.Bar().{method}(ex := New Exception())
            substitute.Bar().{method}(Function(callInfo) New Exception())
            substitute.Bar().{method}(createException:= Function(callInfo) New Exception())
            substitute.Bar().{method}(
                Function(callInfo)
                    Return New Exception()
                End Function)
            substitute.Bar().{method}(
                createException :=
                Function(callInfo)
                    Return New Exception()
                End Function)
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{updatedMethod}(New Exception())
            substitute.Bar().{updatedMethod}(ex := New Exception())
            substitute.Bar().{updatedMethod}(Function(callInfo) New Exception())
            substitute.Bar().{updatedMethod}(createException:= Function(callInfo) New Exception())
            substitute.Bar().{updatedMethod}(
                Function(callInfo)
                    Return New Exception()
                End Function)
            substitute.Bar().{updatedMethod}(
                createException :=
                Function(callInfo)
                    Return New Exception()
                End Function)
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource);
    }

    public override async Task ReplacesThrowsWithThrowsAsync_WhenUsedInProperty(string method, string updatedMethod)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Property Bar As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar.{method}(New Exception())
            substitute.Bar.{method}(ex := New Exception())
            substitute.Bar.{method}(Function(callInfo) New Exception())
            substitute.Bar.{method}(createException:= Function(callInfo) New Exception())
            substitute.Bar.{method}(
                Function(callInfo)
                    Return New Exception()
                End Function)
            substitute.Bar.{method}(
                createException :=
                Function(callInfo)
                    Return New Exception()
                End Function)
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Property Bar As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar.{updatedMethod}(New Exception())
            substitute.Bar.{updatedMethod}(ex := New Exception())
            substitute.Bar.{updatedMethod}(Function(callInfo) New Exception())
            substitute.Bar.{updatedMethod}(createException:= Function(callInfo) New Exception())
            substitute.Bar.{updatedMethod}(
                Function(callInfo)
                    Return New Exception()
                End Function)
            substitute.Bar.{updatedMethod}(
                createException :=
                Function(callInfo)
                    Return New Exception()
                End Function)
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource);
    }

    public override async Task ReplacesThrowsWithThrowsAsync_WhenUsedInIndexer(string method, string updatedMethod)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute(0).{method}(New Exception())
            substitute(0).{method}(ex := New Exception())
            substitute(0).{method}(Function(callInfo) New Exception())
            substitute(0).{method}(createException:= Function(callInfo) New Exception())
            substitute(0).{method}(
                Function(callInfo)
                    Return New Exception()
                End Function)
            substitute(0).{method}(
                createException :=
                Function(callInfo)
                    Return New Exception()
                End Function)
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute(0).{updatedMethod}(New Exception())
            substitute(0).{updatedMethod}(ex := New Exception())
            substitute(0).{updatedMethod}(Function(callInfo) New Exception())
            substitute(0).{updatedMethod}(createException:= Function(callInfo) New Exception())
            substitute(0).{updatedMethod}(
                Function(callInfo)
                    Return New Exception()
                End Function)
            substitute(0).{updatedMethod}(
                createException :=
                Function(callInfo)
                    Return New Exception()
                End Function)
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource);
    }
}