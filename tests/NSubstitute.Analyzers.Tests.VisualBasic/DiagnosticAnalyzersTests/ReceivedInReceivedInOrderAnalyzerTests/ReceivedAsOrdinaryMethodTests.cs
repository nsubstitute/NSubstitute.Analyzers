using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReceivedInReceivedInOrderAnalyzerTests
{
    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "ReceivedExtensions.Received(Of IFoo)(substitute, Quantity.None())",
        "ReceivedExtensions.Received(Of IFoo)(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.Received(Of IFoo)(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute:= substitute)",
        "SubstituteExtensions.Received(Of IFoo)(substitute)",
        "SubstituteExtensions.Received(Of IFoo)(substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute:= substitute, requiredQuantity:= Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(Of IFoo)(requiredQuantity:= Quantity.None(), substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of IFoo)(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceive(Of IFoo)(substitute)",
        "SubstituteExtensions.DidNotReceive(Of IFoo)(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of IFoo)(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of IFoo)(substitute:= substitute)")]
    public class ReceivedAsOrdinaryMethodTests : ReceivedInReceivedInOrderDiagnosticVerifier
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
                                 [|{method}|].Bar()
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
                                 Dim x = [|{method}|].Bar
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
                                 Dim x = [|{method}|].Bar
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
            {method}.Bar()
            Dim x = {method}(0)
            Dim y = {method}.Foo
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
            "ReceivedExtensions.Received(substitute, Quantity.None())",
            "ReceivedExtensions.Received(substitute:= substitute, x:= Quantity.None())",
            "ReceivedExtensions.Received(x:= Quantity.None(), substitute:= substitute)",
            "ReceivedExtensions.Received(Of Foo)(substitute, Quantity.None())",
            "ReceivedExtensions.Received(Of Foo)(substitute:= substitute, x: =Quantity.None())",
            "ReceivedExtensions.Received(Of Foo)(x:= Quantity.None(), substitute:= substitute)",
            "SubstituteExtensions.Received(substitute, 1, 1)",
            "SubstituteExtensions.Received(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.Received(x:= 1, y:= 1, substitute:= substitute)",
            "SubstituteExtensions.Received(Of Foo)(substitute, 1, 1)",
            "SubstituteExtensions.Received(Of Foo)(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.Received(Of Foo)(x:= 1, y:= 1, substitute:= substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute:= substitute, x:= Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(x:= Quantity.None(), substitute:= substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(substitute:= substitute, x:= Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(Of Foo)(x:= Quantity.None(), substitute:= substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs(x:= 1, y:= 1, substitute:= substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)(substitute, 1, 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceive(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.DidNotReceive(x:= 1, y:= 1, substitute:= substitute)",
            "SubstituteExtensions.DidNotReceive(Of Foo)(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceive(Of Foo)(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.DidNotReceive(Of Foo)(x:= 1, y:= 1, substitute:= substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(x:= 1, y:= 1, substitute:= substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)(substitute:= substitute, x:= 1, y:= 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)(x:= 1, y:= 1, substitute:= substitute)")]
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
            {method}.Bar()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        private static string GetPlainMethodName(string methodName)
        {
            var plainMethodName = methodName.Replace("(Of IFoo)", string.Empty)
                .Replace("(substitute, Quantity.None())", string.Empty)
                .Replace("(substitute:= substitute, requiredQuantity:= Quantity.None())", string.Empty)
                .Replace("(requiredQuantity:= Quantity.None(), substitute:= substitute)", string.Empty)
                .Replace("(substitute:= substitute)", string.Empty)
                .Replace("(substitute)", string.Empty);

            var planMethodNameWithoutNamespace = plainMethodName.Replace("SubstituteExtensions.", string.Empty)
                .Replace("ReceivedExtensions.", string.Empty);

            return planMethodNameWithoutNamespace;
        }

        private async Task VerifyDiagnostic(string source, string methodName)
        {
            var plainMethodName = GetPlainMethodName(methodName);

            await VerifyDiagnostic(source, Descriptor, $"{plainMethodName} method used in Received.InOrder block.");
        }
    }
}
