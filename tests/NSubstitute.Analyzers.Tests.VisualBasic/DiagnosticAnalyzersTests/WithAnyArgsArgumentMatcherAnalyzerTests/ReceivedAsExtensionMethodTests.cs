using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReceivedAsExtensionMethodTests : WithAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}.Foo = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithPropertyNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}.Foo = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}.Foo = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
    public override async Task ReportsNoDiagnostics_WhenAssigningArgMatchersToMemberNotPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Action(Of Integer)
        Default Property Item(ByVal x As Integer?) As Action(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}.Foo = {arg}
            substitute.{method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer?) As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer?) As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Default Property Item(ByVal x As Integer?) As Integer?
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface IFoo
        Property Foo As Action(Of Integer)
        Default Property Item(ByVal x As Integer?) As Action(Of Integer)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}.Foo = {arg}
            substitute.{method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
Interface Foo
    Sub Bar(ByVal x As Integer?)
End Interface

Public Class FooTests
    Public Sub Test()
        Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
        substitute.{method}.Bar({arg})
    End Sub
End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "DidNotReceive()",
        "Received(Quantity.None())",
        "Received()")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Sub Bar(ByVal x As Integer?)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}.Bar({arg})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Sub Bar(ByVal x As Integer?)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}.Bar({arg})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }
}