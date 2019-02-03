using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.UnusedReceivedAnalyzerTests
{
    [CombinatoryData(
        "SubstituteExtensions.Received",
        "SubstituteExtensions.Received(Of Foo)",
        "SubstituteExtensions.ReceivedWithAnyArgs",
        "SubstituteExtensions.ReceivedWithAnyArgs(Of Foo)",
        "SubstituteExtensions.DidNotReceive",
        "SubstituteExtensions.DidNotReceive(Of Foo)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Foo)")]
    public class ReceivedAsOrdinaryMethodTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
        {
            var plainMethodName = method.Replace("(Of Foo)", string.Empty);
            var planMethodNameWithoutNamespace = plainMethodName.Replace("SubstituteExtensions.", string.Empty);
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|{method}(substitute)|]
        End Sub
    End Class
End Namespace
";

            await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{planMethodNameWithoutNamespace}"". Correct: ""{plainMethodName}(sub).SomeCall();"". Incorrect: ""{plainMethodName}(sub);""");
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public Class FooBar
    End Class

    Interface Foo
        Function Bar() As FooBar
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute).Bar()
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Property Bar As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim bar = {method}(substitute).Bar
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess(string method)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
        Default ReadOnly Property Item(ByVal x As Integer) As Integer
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim bar = {method}(substitute)(0)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData(
            "SubstituteExtensions.Received",
            "SubstituteExtensions.Received(Of Func(Of Integer))",
            "SubstituteExtensions.ReceivedWithAnyArgs",
            "SubstituteExtensions.ReceivedWithAnyArgs(Of Func(Of Integer))",
            "SubstituteExtensions.DidNotReceive",
            "SubstituteExtensions.DidNotReceive(Of Func(Of Integer))",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(Of Func(Of Integer))")]
        public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method)
        {
            var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            {method}(substitute)()
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
        {
            var source = $@"Imports System
Imports System.Runtime.CompilerServices

Namespace NSubstitute
    Module SubstituteExtensions
        <Extension()>
        Function Received(Of T As Class)(ByVal substitute As T, ByVal params As Decimal) As T
            Return Nothing
        End Function

        <Extension()>
        Function ReceivedWithAnyArgs(Of T As Class)(ByVal substitute As T, ByVal params As Decimal) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceive(Of T As Class)(ByVal substitute As T, ByVal params As Decimal) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceiveWithAnyArgs(Of T As Class)(ByVal substitute As T, ByVal params As Decimal) As T
            Return Nothing
        End Function

    End Module

    Public Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, 1D)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }
    }
}