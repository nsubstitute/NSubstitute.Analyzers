using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ThrowsAsyncAsExtensionMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData("ThrowsAsyncForAnyArgs(Of Exception)()", "ThrowsAsyncForAnyArgs(New Exception())")]
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
            substitute.Bar({arg}).{method}
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("ThrowsAsync(Of Exception)()", "ThrowsAsync(New Exception())")]
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
            substitute.Bar({arg}).{method}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ThrowsAsyncForAnyArgs(Of Exception)()", "ThrowsAsyncForAnyArgs(New Exception())")]
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
            substitute.Bar({arg}).{method}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ThrowsAsyncForAnyArgs(Of Exception)()", "ThrowsAsyncForAnyArgs(New Exception())")]
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
            substitute({arg}).{method}
        End Sub
    End Class
End Namespace";

        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("ThrowsAsync(Of Exception)()", "ThrowsAsync(New Exception())")]
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
            substitute({arg}).{method}
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ThrowsAsyncForAnyArgs(Of Exception)()", "ThrowsAsyncForAnyArgs(New Exception())")]
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
            substitute({arg}).{method}
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}