using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.UnusedReceivedAnalyzerTests
{
    [CombinatoryData("Received", "ReceivedWithAnyArgs", "DidNotReceive", "DidNotReceiveWithAnyArgs")]
    public class ReceivedAsExtensionMethodTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
        {
            var plainMethodName = method.Replace("(Of Foo)", string.Empty);
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface Foo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            [|substitute.{method}()|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{plainMethodName}"". Correct: ""sub.{plainMethodName}().SomeCall();"". Incorrect: ""sub.{plainMethodName}();""");
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
            substitute.{method}().Bar()
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
            Dim bar = substitute.{method}().Bar
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
            Dim bar = substitute.{method}()(0)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData("Received", "ReceivedWithAnyArgs", "DidNotReceive", "DidNotReceiveWithAnyArgs")]
        public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method)
        {
            var source = $@"Imports NSubstitute
Imports System

Namespace MyNamespace

    Public Class FooTests

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Func(Of Integer))()
            substitute.{method}()()
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
            substitute.{method}(1D)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }
    }
}