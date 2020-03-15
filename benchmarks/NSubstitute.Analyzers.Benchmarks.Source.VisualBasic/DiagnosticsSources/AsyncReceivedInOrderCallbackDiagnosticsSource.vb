Imports System.Threading.Tasks

Namespace DiagnosticsSources
    Public Class AsyncReceivedInOrderCallbackDiagnosticsSource
        Public Sub NS5002_AsyncCallbackUsedInReceivedInOrderMethod()
            NSubstitute.Received.InOrder(Async Sub()
                Await Task.CompletedTask
            End Sub)
        End Sub
    End Class
End Namespace
