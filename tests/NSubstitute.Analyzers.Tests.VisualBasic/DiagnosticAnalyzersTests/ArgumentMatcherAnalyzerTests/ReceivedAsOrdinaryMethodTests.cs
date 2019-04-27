using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
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
    public class ReceivedAsOrdinaryMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute).Bar({arg})
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = {method}(substitute)({arg})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
        {
            var source = $@"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute, 1D).Bar({arg})
        End Sub
    End Class

    Module SubstituteExtensions
        <Extension()>
        Function Received(Of T)(ByVal returnValue As T, ByVal x As Decimal) As T
            Return Nothing
        End Function

        <Extension()>
        Function ReceivedWithAnyArgs(Of T)(ByVal returnValue As T, ByVal x As Decimal) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceive(Of T)(ByVal returnValue As T, ByVal x As Decimal) As T
            Return Nothing
        End Function

        <Extension()>
        Function DidNotReceiveWithAnyArgs(Of T)(ByVal returnValue As T, ByVal x As Decimal) As T
            Return Nothing
        End Function
    End Module
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }
    }
}