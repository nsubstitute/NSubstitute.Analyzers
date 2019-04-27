using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("When", "WhenForAnyArgs")]
    public class WhenAsExtensionMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(Function(ByVal x As Foo)
                x.Bar({arg})
            End Function).[Do](Function(x)
                Throw New NullReferenceException()
            End Function)
            substitute.{method}(AddressOf SubstituteCall).[Do](Function(x)
                Throw New NullReferenceException()
            End Function)
        End Sub

        Private Function SubstituteCall(ByVal obj As Foo) As Task
            obj.Bar({arg})
            Return Task.CompletedTask
        End Function
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Public MustInherit Class Foo
        Default Public MustOverride ReadOnly Property Item(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(Function(ByVal x As Foo)
                Dim y = x({arg})
            End Function).[Do](Function(x)
                Throw New NullReferenceException()
            End Function)
            substitute.{method}(AddressOf SubstituteCall).[Do](Function(x)
                Throw New NullReferenceException()
            End Function)
        End Sub

        Private Function SubstituteCall(ByVal obj As Foo) As Task
            Dim x = obj({arg})
            Return Task.CompletedTask
        End Function
    End Class
End Namespace";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
        {
            var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public MustInherit Class Foo
        Public MustOverride Function Bar(ByVal x As Integer) As Integer
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.{method}(Function(ByVal x As Foo)
                x.Bar({arg})
            End Function, 1)
            substitute.{method}(Function(x) x.Bar({arg}), 1)
        End Sub

        Private Function SubstituteCall(ByVal obj As Foo) As Task
            obj.Bar({arg})
            Return Task.CompletedTask
        End Function
    End Class

    Module SubstituteExtensions
        <Extension()>
        Function [When](Of T)(ByVal substitute As T, ByVal substituteCall As System.Action(Of T), ByVal x As Integer) As T
            Return Nothing
        End Function

        <Extension()>
        Function [WhenForAnyArgs](Of T)(ByVal substitute As T, ByVal substituteCall As System.Action(Of T), ByVal x As Integer) As T
            Return Nothing
        End Function
    End Module
End Namespace";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }
    }
}