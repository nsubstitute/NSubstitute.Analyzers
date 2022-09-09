using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ThrowsAsOrdinaryMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer?) As Foo
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
        "ExceptionExtensions.Throws(Of Exception)({0})",
        "ExceptionExtensions.Throws(Of Exception)(value:= {0})",
        "ExceptionExtensions.Throws({0}, New Exception())",
        "ExceptionExtensions.Throws(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.Throws(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer?) As Foo
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
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer?) As Foo
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
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer?) As Foo
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
        "ExceptionExtensions.Throws(Of Exception)({0})",
        "ExceptionExtensions.Throws(Of Exception)(value:= {0})",
        "ExceptionExtensions.Throws({0}, New Exception())",
        "ExceptionExtensions.Throws(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.Throws(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer?) As Foo
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
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)({0})",
        "ExceptionExtensions.ThrowsForAnyArgs(Of Exception)(value:= {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value:= {0}, ex:= New Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex:= New Exception(), value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer?) As Foo
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