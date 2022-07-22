using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.SyncOverAsyncThrowsCodeFixProviderTests;

public class ThrowsAsOrdinaryMethodWithGenericTypeSpecifiedTests : SyncOverAsyncThrowsCodeFixVerifier
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
            ExceptionExtensions.{method}(Of Exception)(substitute.Bar())
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar())
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
            ExceptionExtensions.{method}(Of Exception)(substitute.Bar)
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar)
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
            ExceptionExtensions.{method}(Of Exception)(substitute(0))
            ExceptionExtensions.{method}(Of Exception)(value := substitute(0))
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
            ExceptionExtensions.{method}(Of Exception)(substitute.Bar())
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar())
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
            ExceptionExtensions.{updatedMethod}(Of Exception)(substitute.Bar())
            ExceptionExtensions.{updatedMethod}(Of Exception)(value := substitute.Bar())
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
            ExceptionExtensions.{method}(Of Exception)(substitute.Bar)
            ExceptionExtensions.{method}(Of Exception)(value := substitute.Bar)
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
            ExceptionExtensions.{updatedMethod}(Of Exception)(substitute.Bar)
            ExceptionExtensions.{updatedMethod}(Of Exception)(value := substitute.Bar)
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
            ExceptionExtensions.{method}(Of Exception)(substitute(0))
            ExceptionExtensions.{method}(Of Exception)(value := substitute(0))
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
            ExceptionExtensions.{updatedMethod}(Of Exception)(substitute(0))
            ExceptionExtensions.{updatedMethod}(Of Exception)(value := substitute(0))
        End Sub
    End Class
End Namespace";

        await VerifyFix(source, newSource);
    }
}