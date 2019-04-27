using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("Received.InOrder")]
    public class ReceivedInOrderMethodTests : ArgumentMatcherDiagnosticVerifier
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
            NSubstitute.{method}(Function() substitute.Bar({arg}))
            NSubstitute.{method}(Function()
                substitute.Bar({arg})
            End Function)
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
            NSubstitute.{method}(Function()
                Dim x = substitute({arg})
            End Function)
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

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            MyNamespace.{method}(Sub()
                Dim x = substitute({arg})
            End Sub, 1)
        End Sub
    End Class

    Public Class Received
        Public Shared Sub InOrder(ByVal calls As Action, ByVal x As Integer)
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }
    }
}