Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models
Imports NSubstitute.Core

Namespace DiagnosticsSources
    Public Class ReEntrantSetupDiagnosticsSource
        Public Sub NS4000_ReEntrantSubstituteCall()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.CallInfoReturningMethod().Returns(NSubstitute.Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo)))
            substitute.ConfiguredCallReturningProperty.Returns(NSubstitute.Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo)))
            substitute(0, 0, 0, 0).Returns(NSubstitute.Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo)))
            substitute.CallInfoReturningMethod().Returns(ReEntrantReturn())
            substitute.ConfiguredCallReturningProperty.Returns(ReEntrantReturn())
            substitute(0, 0, 0, 0).Returns(ReEntrantReturn())
            SubstituteExtensions.Returns(substitute.CallInfoReturningMethod(), NSubstitute.Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo)))
            SubstituteExtensions.Returns(substitute.ConfiguredCallReturningProperty, NSubstitute.Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo)))
            SubstituteExtensions.Returns(substitute(0, 0, 0, 0), NSubstitute.Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo)))
            SubstituteExtensions.Returns(substitute.CallInfoReturningMethod(), ReEntrantReturn())
            SubstituteExtensions.Returns(substitute.ConfiguredCallReturningProperty, ReEntrantReturn())
            SubstituteExtensions.Returns(substitute(0, 0, 0, 0), ReEntrantReturn())
        End Sub

        Private Shared Function ReEntrantReturn() As ConfiguredCall
            Return Substitute.[For](Of IFoo)().ObjectReturningMethod().Returns(CType(Nothing, IFoo))
        End Function
    End Class
End Namespace
