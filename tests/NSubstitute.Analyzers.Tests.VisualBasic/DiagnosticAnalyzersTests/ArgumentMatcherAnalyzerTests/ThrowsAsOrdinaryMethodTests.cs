using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("ExceptionExtensions.Throws", "ExceptionExtensions.ThrowsForAnyArgs")]
    public class ThrowsAsOrdinaryMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Object) As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar({arg}), New Exception())
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Object) As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute({arg}), New Exception())
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
        {
            var source = $@"Imports System
Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer) As Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            {method}(substitute.Bar({arg}), New Exception())
        End Sub
    End Class

    Module ExceptionExtensions
        <Extension()>
        Function Throws(Of T As Exception)(ByVal value As Object, ByVal ex As T) As T
            Return Nothing
        End Function

        <Extension()>
        Function ThrowsForAnyArgs(Of T As Exception)(ByVal value As Object, ByVal ex As T) As T
            Return Nothing
        End Function
    End Module
End Namespace
";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }
    }
}