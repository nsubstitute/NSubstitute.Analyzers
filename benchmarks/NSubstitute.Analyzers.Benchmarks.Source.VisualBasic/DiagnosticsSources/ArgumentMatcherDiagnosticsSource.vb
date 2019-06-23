Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models

Namespace DiagnosticsSources
    Public Class ArgumentMatcherDiagnosticsSource
        Public Sub NS5001_ArgumentMatcherUsedWithoutSpecifyingCall()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)())
            substitute.ObjectReturningMethodWithArguments(Arg.Compat.Any(Of Integer)(), Arg.Compat.Any(Of Integer)(), Arg.Compat.Any(Of Decimal)())
            substitute.ObjectReturningMethodWithArguments(Arg.[Is](0), Arg.[Is](0), Arg.[Is](0D))
            substitute.ObjectReturningMethodWithArguments(Arg.Compat.[Is](0), Arg.Compat.[Is](0), Arg.Compat.[Is](0D))
        End Sub
    End Class
End Namespace
