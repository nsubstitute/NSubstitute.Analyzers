Imports NSubstitute.Analyzers.Benchmarks.Shared.Models

Namespace DiagnosticsSources
    Public Class UnusedReceivedDiagnosticsSource
        Public Sub NS5000_ReceivedCheck()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Received(1)
            SubstituteExtensions.Received(substitute, 1)
        End Sub
    End Class
End Namespace
