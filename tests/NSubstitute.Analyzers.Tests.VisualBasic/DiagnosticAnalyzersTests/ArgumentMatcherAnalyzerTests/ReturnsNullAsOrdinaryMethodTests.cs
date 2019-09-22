using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData(
        "ReturnsExtensions.ReturnsNull",
        "ReturnsExtensions.ReturnsNull(Of Object)",
        "ReturnsExtensions.ReturnsNullForAnyArgs",
        "ReturnsExtensions.ReturnsNullForAnyArgs(Of Object)")]
    public class ReturnsNullAsOrdinaryMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Object) As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar({arg}))
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"Imports NSubstitute
Imports NSubstitute.ReturnsExtensions

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Object) As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute({arg}))
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
        Public MustOverride Function Bar(ByVal x As Integer) As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar({arg}))
        End Sub
    End Class

    Module ReturnsExtensions
        <Extension()>
        Function ReturnsNull(Of T)(ByVal returnValue As T) As T
            Return Nothing
        End Function

        <Extension()>
        Function ReturnsNullForAnyArgs(Of T)(ByVal returnValue As T) As T
            Return Nothing
        End Function
    End Module
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }
    }
}