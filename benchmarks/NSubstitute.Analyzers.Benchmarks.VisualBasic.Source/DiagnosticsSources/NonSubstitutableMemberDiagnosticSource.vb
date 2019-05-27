Imports NSubstitute.Analyzers.Benchmarks.VisualBasic.Source.Models

Namespace DiagnosticsSources
    Public Class NonSubstitutableMemberDiagnosticSource
        Public Sub NS1000_NonVirtualSetupSpecification()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.ObjectReturningMethod().Returns(CType(Nothing, IFoo))
            substitute.[Property].Returns(1)
            substitute(0).Returns(1)
            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), CType(Nothing, IFoo))
            SubstituteExtensions.Returns(substitute.[Property], 1)
            SubstituteExtensions.Returns(substitute(0), 1)
            SubstituteExtensions.Returns(1, 1)
        End Sub

        Public Sub NS1003_InternalVirtualSetupSpecification()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.InternalObjectReturningMethod().Returns(CType(Nothing, IFoo))
            substitute.InternalObjectReturningProperty.Returns(CType(Nothing, IFoo))
            substitute(0, 0, 0).Returns(CType(Nothing, IFoo))
            SubstituteExtensions.Returns(substitute.InternalObjectReturningMethod(), CType(Nothing, IFoo))
            SubstituteExtensions.Returns(substitute.InternalObjectReturningProperty, CType(Nothing, IFoo))
            SubstituteExtensions.Returns(substitute(0, 0, 0), CType(Nothing, IFoo))
        End Sub
    End Class
End Namespace
