Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models

Namespace DiagnosticsSources
    Public Class ReceivedLikeUsedInReceivedInOrderDiagnosticSource
        Public Sub NS5001_ReceivedLikeUsedInReceivedInOrderCallback()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            NSubstitute.Received.InOrder(Function()
                substitute.Received().ObjectReturningMethod()
                Dim x = substitute.Received().InternalObjectReturningProperty
                Dim y = substitute.Received()(0)
                SubstituteExtensions.Received(substitute).ObjectReturningMethod()
                Dim a = SubstituteExtensions.Received(substitute).InternalObjectReturningProperty
                Dim b = SubstituteExtensions.Received(substitute)(0)
                Throw New System.Exception()
            End Function)
        End Sub
    End Class
End Namespace
