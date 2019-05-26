Imports System.Threading.Tasks
Imports NSubstitute.Analyzers.Benchmarks.Shared.Models

Namespace DiagnosticsSources
    Public Class NonSubstitutableMemberWhenDiagnosticsSource
        Public Sub NS1002_NonVirtualSetupSpecification()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.[When](Function([sub]) [sub].ObjectReturningMethod())
            substitute.[When](Function([sub])
                                  Dim a = [sub].[Property]
                                  Return Task.CompletedTask
                              End Function)
            substitute.[When](Function([sub])
                                  Dim a = [sub](0)
                                  Return Task.CompletedTask
                              End Function)
            substitute.[When](AddressOf WhenDelegate)
            SubstituteExtensions.[When](substitute, Function([sub]) [sub].ObjectReturningMethod())
            SubstituteExtensions.[When](substitute, Function([sub])
                                                        Dim a = [sub].[Property]
                                                        Return Task.CompletedTask
                                                    End Function)
            SubstituteExtensions.[When](substitute, Function([sub])
                                                        Dim a = [sub](0)
                                                        Return Task.CompletedTask
                                                    End Function)
            SubstituteExtensions.[When](substitute, AddressOf WhenDelegate)
        End Sub

        Public Sub NS1003_InternalVirtualSetupSpecification()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.[When](Function([sub]) [sub].InternalObjectReturningMethod())
            substitute.[When](Function([sub])
                                  Dim a = [sub].InternalObjectReturningProperty
                                  Return Task.CompletedTask
                              End Function)
            substitute.[When](Function([sub])
                                  Dim a = [sub](0, 0, 0)
                                  Return Task.CompletedTask
                              End Function)
            substitute.[When](AddressOf WhenDelegateWithInternal)
            SubstituteExtensions.[When](substitute, Function([sub]) [sub].InternalObjectReturningMethod())
            SubstituteExtensions.[When](substitute, Function([sub])
                                                        Dim a = [sub].InternalObjectReturningProperty
                                                        Return Task.CompletedTask
                                                    End Function)
            SubstituteExtensions.[When](substitute, Function([sub])
                                                        Dim a = [sub](0, 0, 0)
                                                        Return Task.CompletedTask
                                                    End Function)
            SubstituteExtensions.[When](substitute, AddressOf WhenDelegateWithInternal)
        End Sub

        Public Function WhenDelegate(ByVal foo As Foo) As Task
            foo.ObjectReturningMethod()
            Return Task.CompletedTask
        End Function

        Public Function WhenDelegateWithInternal(ByVal foo As Foo) As Task
            foo.InternalObjectReturningMethod()
            Return Task.CompletedTask
        End Function
    End Class
End Namespace
