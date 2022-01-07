using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReceivedInReceivedInOrderAnalyzerTests;

[CombinatoryData(
    "Received(Quantity.None())",
    "Received()",
    "ReceivedWithAnyArgs(Quantity.None())",
    "ReceivedWithAnyArgs()",
    "DidNotReceive()",
    "DidNotReceiveWithAnyArgs()")]
public class ReceivedAsExtensionMethodTests : ReceivedInReceivedInOrderDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForMethod(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Public Interface IFoo
        Function Bar() As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 [|substitute.{method}|].Bar()
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, method);
    }

    public override async Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForProperty(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Public Interface IFoo
         ReadOnly Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 Dim x = [|substitute.{method}|].Bar
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, method);
    }

    public override async Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForIndexer(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Public Interface IFoo
        ReadOnly Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            NSubstitute.Received.InOrder(Function()
                                 Dim x = [|substitute.{method}|].Bar
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyDiagnostic(source, method);
    }

    public override async Task ReportsNoDiagnostic_WhenUsingReceivedLikeMethodOutsideOfReceivedInOrderBlock(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Public Interface IFoo
        Function Bar() As Integer
        ReadOnly Property Foo As Integer
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.{method}.Bar()
            Dim x = substitute.{method}(0)
            Dim y = substitute.{method}.Foo
            NSubstitute.Received.InOrder(Function()
                                 Dim a = substitute(0)
                                 Dim b = substitute.Foo
                                 substitute.Bar()
                             End Function)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received(1, 1)",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs(1, 1)",
        "DidNotReceive(1, 1)",
        "DidNotReceiveWithAnyArgs(1, 1)")]
    public override async Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method)
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
        Public Function Bar() As Integer
            Return 1
        End Function
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
            substitute.{method}.Bar()
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    private static string GetPlainMethodName(string methodName)
    {
        return methodName.Replace("<IFoo>", string.Empty)
            .Replace("Quantity.None()", string.Empty)
            .Replace("()", string.Empty);
    }

    private async Task VerifyDiagnostic(string source, string methodName)
    {
        var plainMethodName = GetPlainMethodName(methodName);

        await VerifyDiagnostic(source, Descriptor, $"{plainMethodName} method used in Received.InOrder block.");
    }
}