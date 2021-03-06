Imports System
Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models

Namespace DiagnosticsSources
    Public Class NonSubstitutableMemberArgumentMatcherDiagnosticsSource
        Public Sub NS1004_ArgumentMatcherUsedWithNonVirtualMember()
            Dim substitute = NSubstitute.Substitute.[For] (Of Foo)()
            substitute.ObjectReturningMethodWithArguments(Arg.Any (Of Integer)(), Arg.Compat.Any (Of Integer)(),
                                                          Arg.[Is](1D))
            substitute.ObjectReturningMethodWithArguments(
                Arg.[Do] (Of Integer)(Function(i)
                    Throw New Exception
                End Function),
                Arg.Compat.[Do] (Of Integer)(Function(i)
                    Throw New Exception
                End Function), Arg.Compat.[Is](1D))
            substitute.ObjectReturningMethodWithArguments(Arg.Invoke(1), Arg.InvokeDelegate (Of Integer)(),
                                                          Arg.Compat.InvokeDelegate (Of Integer)())
            substitute.ObjectReturningMethodWithArguments(Arg.Compat.Invoke(1),
                                                          Arg.Compat.[Do] (Of Integer)(Function(i)
                                                              Throw New Exception
                                                          End Function), Arg.Compat.[Is](1D))
            substitute.InternalObjectReturningMethodWithArguments(Arg.[Is](1))
            substitute.InternalObjectReturningMethodWithArguments(Arg.Compat.[Is](1))
            substitute.InternalObjectReturningMethodWithArguments(Arg.Any (Of Integer)())
            substitute.InternalObjectReturningMethodWithArguments(Arg.Compat.Any (Of Integer)())
            Dim first = substitute(Arg.Any (Of Integer)())
            Dim second = substitute(Arg.Compat.Any (Of Integer)())
        End Sub
    End Class
End Namespace
