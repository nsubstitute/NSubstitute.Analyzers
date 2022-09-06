using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.UnusedReceivedAnalyzerTests;

[CombinatoryData(
    "ReceivedExtensions.Received(substitute, Quantity.None())",
    "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
    "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
    "ReceivedExtensions.Received(Of Foo)(substitute, Quantity.None())",
    "ReceivedExtensions.Received(Of Foo)(substitute:= substitute, requiredQuantity:= Quantity.None())",
    "ReceivedExtensions.Received(Of Foo)(requiredQuantity:= Quantity.None(), substitute:= substitute)",
    "SubstituteExtensions.Received(substitute)",
    "SubstituteExtensions.Received(substitute:= substitute)",
    "SubstituteExtensions.Received(Of Foo)(substitute)",
    "SubstituteExtensions.Received(Of Foo)(substitute:= substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(substitute:= substitute, requiredQuantity:= Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(requiredQuantity:= Quantity.None(), substitute:= substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)(substitute:= substitute)",
    "SubstituteExtensions.DidNotReceive(substitute)",
    "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
    "SubstituteExtensions.DidNotReceive(Of Foo)(substitute)",
    "SubstituteExtensions.DidNotReceive(Of Foo)(substitute:= substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)(substitute:= substitute)")]
public class ReceivedAsOrdinaryMethodTests : UnusedReceivedDiagnosticVerifier
{
    public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
    {
        var plainMethodName = method.Replace("(Of Foo)", string.Empty)
            .Replace("(substitute, Quantity.None())", string.Empty)
            .Replace("(substitute:= substitute, requiredQuantity:= Quantity.None())", string.Empty)
            .Replace("(requiredQuantity:= Quantity.None(), substitute:= substitute)", string.Empty)
            .Replace("(substitute:= substitute)", string.Empty)
            .Replace("(substitute)", string.Empty);

        var planMethodNameWithoutNamespace = plainMethodName.Replace("SubstituteExtensions.", string.Empty)
            .Replace("ReceivedExtensions.", string.Empty);

        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|{method}|]
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{planMethodNameWithoutNamespace}"". Correct: ""{plainMethodName}(sub).SomeCall();"". Incorrect: ""{plainMethodName}(sub);""");
    }

    public override async Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Public Class FooBar
    End Class

    Interface Foo
        Function Bar() As FooBar
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}.Bar()
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim bar = {method}.Bar
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim bar = {method}(0)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "ReceivedExtensions.Received(Of Func(Of Integer))(substitute, Quantity.None())",
        "ReceivedExtensions.Received(Of Func(Of Integer))(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(Of Func(Of Integer))(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute:= substitute)",
        "SubstituteExtensions.Received(Of Func(Of Integer))(substitute)",
        "SubstituteExtensions.Received(Of Func(Of Integer))(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceive(Of Func(Of Integer))(substitute)",
        "SubstituteExtensions.DidNotReceive(Of Func(Of Integer))(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Func(Of Integer))(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Func(Of Integer))(substitute:= substitute)")]
    public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            {method}()
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(Of Foo)(substitute, Quantity.None())",
        "SubstituteExtensions.Received(substitute, 1, 1)",
        "SubstituteExtensions.Received(Of Foo)(substitute, 1, 1)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(substitute, Quantity.None())",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive(Of Foo)(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)(substitute, 1, 1)")]
    public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
    {
        var source = $@"Imports System
Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Public Class Quantity
        Public Shared Function None() As Quantity
            Return Nothing
        End Function
    End Class

    Public Class Foo
    End Class

    Module SubstituteExtensions
        <Extension()>
        Function Received(Of T)(ByVal substitute As T, ByVal x As Integer, ByVal y As Integer) As T
            Return Nothing
        End Function

        <Extension()>
        Function ReceivedWithAnyArgs(Of T)(ByVal substitute As T, ByVal x As Integer, ByVal y As Integer) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceive(Of T)(ByVal substitute As T, ByVal x As Integer, ByVal y As Integer) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceiveWithAnyArgs(Of T)(ByVal substitute As T, ByVal x As Integer, ByVal y As Integer) As T
            Return Nothing
        End Function
    End Module

    Module ReceivedExtensions
        <Extension()>
        Function Received(Of T)(ByVal substitute As T, ByVal x As Quantity) As T
            Return Nothing
        End Function

        <Extension()>
        Function ReceivedWithAnyArgs(Of T)(ByVal substitute As T, ByVal x As Quantity) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceive(Of T)(ByVal substitute As T, ByVal x As Quantity) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceiveWithAnyArgs(Of T)(ByVal substitute As T, ByVal x As Quantity) As T
            Return Nothing
        End Function
    End Module

    Public Class FooTests
        Public Sub Test()
            Dim substitute As Foo = Nothing
            {method}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }
}