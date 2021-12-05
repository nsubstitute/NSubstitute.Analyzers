using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.AsyncThrowsAnalyzerTests
{
    public class ThrowsAsExtensionMethodTests : AsyncThrowsDiagnosticVerifier
    {
        public override async Task ReportsDiagnostic_WhenUsedWithNonGenericAsyncMethod(string method)
        {
            var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            [|substitute.Bar().{method}(New Exception())|]
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, AsyncThrowsDescriptor);
        }

        public override async Task ReportsDiagnostic_WhenUsedWithGenericAsyncMethod(string method)
        {
            var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Task(Of Object)
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            [|substitute.Bar().{method}(New Exception())|]
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, AsyncThrowsDescriptor);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedWithSyncMethod(string method)
        {
            var source = $@"Imports System
Imports System.Threading.Tasks
Imports NSubstitute
Imports NSubstitute.ExceptionExtensions

Namespace MyNamespace
    Interface IFoo
        Function Bar() As Object
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Bar().{method}(New Exception())
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }
    }
}