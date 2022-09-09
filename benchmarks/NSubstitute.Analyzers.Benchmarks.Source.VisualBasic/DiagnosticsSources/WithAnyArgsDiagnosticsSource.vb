Imports System
Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models

Namespace DiagnosticsSources
    Public Class WithAnyArgsDiagnosticsSource
        Public Sub NS5004_InvalidArgumentMatcherUsedWithAnyArgs()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            Dim fist = substitute.DidNotReceiveWithAnyArgs()(Arg.[Is](1))
            Dim second = substitute.DidNotReceiveWithAnyArgs()(Arg.[Do](Of Integer)(Function(x)
                                                                                        Throw New Exception
                                                                                    End Function))
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.[Is](1)
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.[Do](Of Integer)(Function(x)
                                                                                                    Throw New Exception
                                                                                              End Function)
            substitute.DidNotReceiveWithAnyArgs().ObjectReturningMethodWithArguments(Arg.[Is](1), Arg.[Is](1), Arg.[Do](Of Integer)(Function(x)
                                                                                                                                            Throw New Exception
                                                                                                                                    End Function))
            Dim third = substitute.DidNotReceiveWithAnyArgs()(Arg.[Is](1))
            Dim fourth = substitute.DidNotReceiveWithAnyArgs()(Arg.[Do](Of Integer)(Function(x)
                                                                                        Throw New Exception()
                                                                                    End Function))
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.[Is](1)
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.[Do](Of Integer)(Function(x)
                                                                                                Throw New Exception()
                                                                                              End Function)
            substitute.DidNotReceiveWithAnyArgs().ObjectReturningMethodWithArguments(Arg.[Is](1), Arg.[Is](1), Arg.[Do](Of Integer)(Function(x)
                                                                                                                                            Throw New Exception()
                                                                                                                                    End Function))
            Dim fifth = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)(Arg.[Is](1))
            Dim sixth = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)(Arg.[Do](Of Integer)(Function(x)
                                                                                                        Throw New Exception()
                                                                                                       End Function))
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.[Is](1)
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.[Do](Of Integer)(Function(x)
                                                                                                                    Throw New Exception()
                                                                                                                  End Function)
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).ObjectReturningMethodWithArguments(Arg.[Is](1), Arg.[Is](1), Arg.[Do](Of Integer)(Function(x)
                                                                                                                                                                Throw New Exception()
                                                                                                                                                        End Function))
            Dim seventh = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)(Arg.[Is](1))
            Dim eigth = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)(Arg.[Do](Of Integer)(Function(x)
                                                                                                        Throw New Exception()
                                                                                                       End Function))
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.[Is](1)
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.[Do](Of Integer)(Function(x)
                                                                                                                    Throw New Exception()
                                                                                                                  End Function)
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).ObjectReturningMethodWithArguments(Arg.[Is](1), Arg.[Is](1), Arg.[Do](Of Integer)(Function(x)
                                                                                                                                                                Throw New Exception()
                                                                                                                                                        End Function))
        End Sub
    End Class
End Namespace
