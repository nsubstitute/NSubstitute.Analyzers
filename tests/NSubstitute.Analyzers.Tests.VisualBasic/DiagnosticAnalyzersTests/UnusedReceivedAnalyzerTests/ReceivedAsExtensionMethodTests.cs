using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.UnusedReceivedAnalyzerTests;

[CombinatoryData(
    "Received(Quantity.None())",
    "Received()",
    "ReceivedWithAnyArgs(Quantity.None())",
    "ReceivedWithAnyArgs()",
    "DidNotReceive()",
    "DidNotReceiveWithAnyArgs()")]
public class ReceivedAsExtensionMethodTests : UnusedReceivedDiagnosticVerifier
{
    public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
    {
        var plainMethodName = method.Replace("(Of Foo)", string.Empty)
            .Replace("Quantity.None()", string.Empty)
            .Replace("()", string.Empty);

        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions

Namespace MyNamespace
    Interface Foo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|substitute.{method}|]
        End Sub
    End Class
End Namespace
";
        await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{plainMethodName}"". Correct: ""sub.{plainMethodName}().SomeCall();"". Incorrect: ""sub.{plainMethodName}();""");
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
            substitute.{method}.Bar()
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
            Dim bar = substitute.{method}.Bar
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
            Dim bar = substitute.{method}(0)
        End Sub
    End Class
End Namespace
";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received()",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "DidNotReceive()",
        "DidNotReceiveWithAnyArgs()")]
    public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method)
    {
        var source = $@"Imports NSubstitute
Imports NSubstitute.ReceivedExtensions
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            substitute.{method}()
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
            substitute.{method}
        End Sub
    End Class
End Namespace
";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSubscribingToEvent(string method)
    {
        var source = @$"Imports NSubstitute
Imports System
Imports NSubstitute.ReceivedExtensions
Imports NUnit.Framework

Namespace MyNamespace
    Public Class Foo
        Public Event SomeEvent As Action
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For] (Of Foo)()
            AddHandler substitute.{method}.SomeEvent, Arg.Any (Of Action)()
        End Sub
    End Class
End Namespace";

        await VerifyNoDiagnostic(source);
    }
}