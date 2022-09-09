using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReceivedAsOrdinaryMethodTests : WithAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
            {method}.Foo = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute:= substitute)")]
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
            {method}.Foo = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
            {method}.Foo = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute:= substitute)")]
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
            {method}.Foo = {arg}
            {method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
            {method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute:= substitute)")]
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
            {method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
            {method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
            {method}.Foo = {arg}
            {method}(1) = {arg}
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
        {method}.Bar({arg})
    End Sub
End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute:= substitute)")]
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
            {method}.Bar({arg})
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)")]
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
            {method}.Bar({arg})
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }
}