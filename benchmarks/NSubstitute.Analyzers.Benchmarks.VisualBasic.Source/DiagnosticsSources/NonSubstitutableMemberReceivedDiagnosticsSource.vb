Imports NSubstitute.Analyzers.Benchmarks.Shared.Models

Namespace DiagnosticsSources
    Public Class NonSubstitutableMemberReceivedDiagnosticsSource
        Public Sub NS1001_NonVirtualSetupSpecification()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Received(1).ObjectReturningMethod()
            Dim a = substitute.Received(1).[Property]
            Dim b = substitute.Received(1)(0)
            SubstituteExtensions.Received(substitute, 1).ObjectReturningMethod()
            Dim c  = SubstituteExtensions.Received(substitute, 1).[Property]
            Dim d = SubstituteExtensions.Received(substitute, 1)(0)
        End Sub

        Public Sub NS1003_InternalVirtualSetupSpecification()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.Received(1).InternalObjectReturningMethod()
            Dim a = substitute.Received(1).InternalObjectReturningProperty
            Dim b = substitute.Received(1)(0, 0, 0)
            SubstituteExtensions.Received(substitute, 1).InternalObjectReturningMethod()
            Dim c = SubstituteExtensions.Received(substitute, 1).InternalObjectReturningProperty
            Dim d = SubstituteExtensions.Received(substitute, 1)(0, 0, 0)
        End Sub
    End Class
End Namespace
