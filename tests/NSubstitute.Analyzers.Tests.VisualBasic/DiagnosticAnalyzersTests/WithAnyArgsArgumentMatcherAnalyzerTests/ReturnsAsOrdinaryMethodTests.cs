using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReturnsAsOrdinaryMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "SubstituteExtensions.ReturnsForAnyArgs({0}, CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(value:= {0}, returnThis:= CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis:= CType(Nothing, Foo), value:= {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value:= {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Foo
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
        "SubstituteExtensions.Returns({0}, CType(Nothing, Foo))",
        "SubstituteExtensions.Returns(value:= {0}, returnThis:= CType(Nothing, Foo))",
        "SubstituteExtensions.Returns(returnThis:= CType(Nothing, Foo), value:= {0})",
        "ReturnsExtensions.ReturnsNull({0})",
        "ReturnsExtensions.ReturnsNull(value:= {0})",
        "ReturnsForAllExtensions.ReturnsForAll({0}, CType(Nothing, Foo))",
        "ReturnsForAllExtensions.ReturnsForAll(substitute:= {0}, returnThis:= CType(Nothing, Foo))",
        "ReturnsForAllExtensions.ReturnsForAll(returnThis:= CType(Nothing, Foo), substitute:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports NSubstitute.Extensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Foo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute.Bar({arg})")}
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.ReturnsForAnyArgs({0}, CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(value:= {0}, returnThis:= CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis:= CType(Nothing, Foo), value:= {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports NSubstitute.Extensions

Namespace MyNamespace
    Interface Foo
        Function Bar(ByVal x As Integer) As Foo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {string.Format(method, $"substitute.Bar({arg})")}
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.ReturnsForAnyArgs({0}, CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(value:= {0}, returnThis:= CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis:= CType(Nothing, Foo), value:= {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value:= {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

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
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "SubstituteExtensions.Returns({0}, CType(Nothing, Foo))",
        "SubstituteExtensions.Returns(value:= {0}, returnThis:= CType(Nothing, Foo))",
        "SubstituteExtensions.Returns(returnThis:= CType(Nothing, Foo), value:= {0})",
        "ReturnsExtensions.ReturnsNull({0})",
        "ReturnsExtensions.ReturnsNull(value:= {0})",
        "ReturnsForAllExtensions.ReturnsForAll({0}, CType(Nothing, Foo))",
        "ReturnsForAllExtensions.ReturnsForAll(substitute:= {0}, returnThis:= CType(Nothing, Foo))",
        "ReturnsForAllExtensions.ReturnsForAll(returnThis:= CType(Nothing, Foo), substitute:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports NSubstitute.Extensions

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
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.ReturnsForAnyArgs({0}, CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(value:= {0}, returnThis:= CType(Nothing, Foo))",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis:= CType(Nothing, Foo), value:= {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value:= {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReturnsExtensions
Imports NSubstitute.Extensions

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
End Namespace
";
        await VerifyNoDiagnostic(source);
    }
}