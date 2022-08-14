using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ThrowsAsyncAsOrdinaryMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer?) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute.Bar({arg})")}
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsync(Of Exception)({0})",
        "ExceptionExtensions.ThrowsAsync(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsAsync({0}, New Exception())",
        "ExceptionExtensions.ThrowsAsync(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsAsync(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer?) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute.Bar({arg})")}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer?) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute.Bar({arg})")}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer?) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute({arg})")}
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsync(Of Exception)({0})",
        "ExceptionExtensions.ThrowsAsync(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsAsync({0}, New Exception())",
        "ExceptionExtensions.ThrowsAsync(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsAsync(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer?) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute({arg})")}
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer?) As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute({arg})")}
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}