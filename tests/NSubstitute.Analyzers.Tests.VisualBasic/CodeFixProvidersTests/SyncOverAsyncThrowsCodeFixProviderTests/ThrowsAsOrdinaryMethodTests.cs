using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class ThrowsAsOrdinaryMethodTests : SyncOverAsyncThrowsCodeFixVerifier
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
            ExceptionExtensions.{method}(substitute.Bar(), New Exception())
            ExceptionExtensions.{method}(value := substitute.Bar(), ex := New Exception())
            ExceptionExtensions.{method}(ex := New Exception(), value := substitute.Bar())
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
            SubstituteExtensions.{updatedMethod}(substitute.Bar(), Task.FromException(New Exception()))
            SubstituteExtensions.{updatedMethod}(substitute.Bar(), Task.FromException(New Exception()))
            SubstituteExtensions.{updatedMethod}(substitute.Bar(), Task.FromException(New Exception()))
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
            ExceptionExtensions.{method}(substitute.Bar, New Exception())
            ExceptionExtensions.{method}(value := substitute.Bar, ex := New Exception())
            ExceptionExtensions.{method}(ex := New Exception(), value := substitute.Bar)
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
            SubstituteExtensions.{updatedMethod}(substitute.Bar, Task.FromException(New Exception()))
            SubstituteExtensions.{updatedMethod}(substitute.Bar, Task.FromException(New Exception()))
            SubstituteExtensions.{updatedMethod}(substitute.Bar, Task.FromException(New Exception()))
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
            ExceptionExtensions.{method}(substitute(0), New Exception())
            ExceptionExtensions.{method}(value := substitute(0), ex := New Exception())
            ExceptionExtensions.{method}(ex := New Exception(), value := substitute(0))
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
            SubstituteExtensions.{updatedMethod}(substitute(0), Task.FromException(New Exception()))
            SubstituteExtensions.{updatedMethod}(substitute(0), Task.FromException(New Exception()))
            SubstituteExtensions.{updatedMethod}(substitute(0), Task.FromException(New Exception()))
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
            ExceptionExtensions.{method}(substitute.Bar(), New Exception())
            ExceptionExtensions.{method}(value := substitute.Bar(), ex := New Exception())
            ExceptionExtensions.{method}(ex := New Exception(), value := substitute.Bar())
            ExceptionExtensions.{method}(substitute.Bar(), Function(callInfo) New Exception())
            ExceptionExtensions.{method}(value:= substitute.Bar(), createException:= Function(callInfo) New Exception())
            ExceptionExtensions.{method}(createException:= Function(callInfo) New Exception(), value:= substitute.Bar())
            ExceptionExtensions.{method}(substitute.Bar(),
                Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{method}(value:= substitute.Bar(),
                createException:= Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{method}(
                createException:= Function(callInfo)
                    Return New Exception()
                End Function, value:= substitute.Bar())
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
            ExceptionExtensions.{updatedMethod}(substitute.Bar(), New Exception())
            ExceptionExtensions.{updatedMethod}(value := substitute.Bar(), ex := New Exception())
            ExceptionExtensions.{updatedMethod}(ex := New Exception(), value := substitute.Bar())
            ExceptionExtensions.{updatedMethod}(substitute.Bar(), Function(callInfo) New Exception())
            ExceptionExtensions.{updatedMethod}(value:= substitute.Bar(), createException:= Function(callInfo) New Exception())
            ExceptionExtensions.{updatedMethod}(createException:= Function(callInfo) New Exception(), value:= substitute.Bar())
            ExceptionExtensions.{updatedMethod}(substitute.Bar(),
                Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{updatedMethod}(value:= substitute.Bar(),
                createException:= Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{updatedMethod}(
                createException:= Function(callInfo)
                    Return New Exception()
                End Function, value:= substitute.Bar())
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
            ExceptionExtensions.{method}(substitute.Bar, New Exception())
            ExceptionExtensions.{method}(value := substitute.Bar, ex := New Exception())
            ExceptionExtensions.{method}(ex := New Exception(), value := substitute.Bar)
            ExceptionExtensions.{method}(substitute.Bar, Function(callInfo) New Exception())
            ExceptionExtensions.{method}(value:= substitute.Bar, createException:= Function(callInfo) New Exception())
            ExceptionExtensions.{method}(createException:= Function(callInfo) New Exception(), value:= substitute.Bar)
            ExceptionExtensions.{method}(substitute.Bar,
                Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{method}(value:= substitute.Bar,
                createException:= Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{method}(
                createException:= Function(callInfo)
                    Return New Exception()
                End Function, value:= substitute.Bar)
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
            ExceptionExtensions.{updatedMethod}(substitute.Bar, New Exception())
            ExceptionExtensions.{updatedMethod}(value := substitute.Bar, ex := New Exception())
            ExceptionExtensions.{updatedMethod}(ex := New Exception(), value := substitute.Bar)
            ExceptionExtensions.{updatedMethod}(substitute.Bar, Function(callInfo) New Exception())
            ExceptionExtensions.{updatedMethod}(value:= substitute.Bar, createException:= Function(callInfo) New Exception())
            ExceptionExtensions.{updatedMethod}(createException:= Function(callInfo) New Exception(), value:= substitute.Bar)
            ExceptionExtensions.{updatedMethod}(substitute.Bar,
                Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{updatedMethod}(value:= substitute.Bar,
                createException:= Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{updatedMethod}(
                createException:= Function(callInfo)
                    Return New Exception()
                End Function, value:= substitute.Bar)
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
            ExceptionExtensions.{method}(substitute(0), New Exception())
            ExceptionExtensions.{method}(value := substitute(0), ex := New Exception())
            ExceptionExtensions.{method}(ex := New Exception(), value := substitute(0))
            ExceptionExtensions.{method}(substitute(0), Function(callInfo) New Exception())
            ExceptionExtensions.{method}(value:= substitute(0), createException:= Function(callInfo) New Exception())
            ExceptionExtensions.{method}(createException:= Function(callInfo) New Exception(), value:= substitute(0))
            ExceptionExtensions.{method}(substitute(0),
                Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{method}(value:= substitute(0),
                createException:= Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{method}(
                createException:= Function(callInfo)
                    Return New Exception()
                End Function, value:= substitute(0))
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
            ExceptionExtensions.{updatedMethod}(substitute(0), New Exception())
            ExceptionExtensions.{updatedMethod}(value := substitute(0), ex := New Exception())
            ExceptionExtensions.{updatedMethod}(ex := New Exception(), value := substitute(0))
            ExceptionExtensions.{updatedMethod}(substitute(0), Function(callInfo) New Exception())
            ExceptionExtensions.{updatedMethod}(value:= substitute(0), createException:= Function(callInfo) New Exception())
            ExceptionExtensions.{updatedMethod}(createException:= Function(callInfo) New Exception(), value:= substitute(0))
            ExceptionExtensions.{updatedMethod}(substitute(0),
                Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{updatedMethod}(value:= substitute(0),
                createException:= Function(callInfo)
                    Return New Exception()
                End Function)
            ExceptionExtensions.{updatedMethod}(
                createException:= Function(callInfo)
                    Return New Exception()
                End Function, value:= substitute(0))
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource);
    }
}