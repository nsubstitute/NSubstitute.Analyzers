Imports NSubstitute.Analyzers.Benchmarks.VisualBasic.Source.Models

Namespace DiagnosticsSources
    Public Class CallInfoDiagnosticsSource
        Public Sub NS3000_UnableToFindMatchingArgument()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()).Returns(Function(callInfo)
                                                                                                                                           callInfo.ArgAt(Of Integer)(10)
                                                                                                                                           Return Nothing
                                                                                                                                       End Function)
            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()), Function(callInfo)
                                                                                                                                                                 callInfo.ArgAt(Of Integer)(10)
                                                                                                                                                                 Return Nothing
                                                                                                                                                             End Function)
        End Sub

        Public Sub NS3001_CouldNotConvertParameter()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()).Returns(Function(callInfo)
                                                                                                                                           Dim a = CDec(callInfo(1))
                                                                                                                                           Dim b = CDec(callInfo.Args()(1))
                                                                                                                                           callInfo.ArgAt(Of Decimal)(1)
                                                                                                                                           Return Nothing
                                                                                                                                       End Function)
            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()), Function(callInfo)
                                                                                                                                                                 Dim a = CDec(callInfo(1))
                                                                                                                                                                 Dim b = CDec(callInfo.Args()(1))
                                                                                                                                                                 callInfo.ArgAt(Of Decimal)(1)
                                                                                                                                                                 Return Nothing
                                                                                                                                                             End Function)
        End Sub

        Public Sub NS3002_CouldNotFindArgumentOfTypeToThisCall()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethod().Returns(Function(callInfo) callInfo.Arg(Of Object)())
            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), Function(callInfo) callInfo.Arg(Of Object)())
        End Sub

        Public Sub NS3003_ThereIsMoreThanOneArgumentOfGivenTypeToThisCall()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()).Returns(Function(callInfo)
                                                                                                                                           callInfo.Arg(Of Integer)()
                                                                                                                                           Return Nothing
                                                                                                                                       End Function)
            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()), Function(callInfo)
                                                                                                                                                                 callInfo.Arg(Of Integer)()
                                                                                                                                                                 Return Nothing
                                                                                                                                                             End Function)
        End Sub

        Public Sub NS3004_CouldNotSetValueOfTypeToArgumentBecauseTypesAreIncompatible()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithRefArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()).Returns(Function(callInfo)
                                                                                                                                            callInfo(0) = "invalid"
                                                                                                                                            Return Nothing
                                                                                                                                        End Function)
            
            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithRefArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()), Function(callInfo)
                callInfo(0) = "invalid"
                Return Nothing
            End Function)
        End Sub

        Public Sub NS3005_CouldNotSetArgumentAsItIsNotRefOrOutArgument()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()).Returns(Function(callInfo)
                                                                                                                                            callInfo(0) =  1
                                                                                                                                            Return Nothing
                                                                                                                                        End Function)
            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()), Function(callInfo)
                                                                                                                                                                callInfo(0) = 1
                                                                                                                                                                Return Nothing
                                                                                                                                                            End Function)
        End Sub

        Public Sub NS3006_ConflictingArgumentAssignments()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.ObjectReturningMethodWithRefArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()).Returns(Function(callInfo)
                callInfo(0) = 1
                Return Nothing
                End Function).AndDoes(Function(callInfo)
                        callInfo(0) = 2
                        Return Nothing
                    End Function)
            
            SubstituteExtensions.Returns(substitute.ObjectReturningMethodWithRefArguments(Arg.Any(Of Integer)(), Arg.Any(Of Integer)(), Arg.Any(Of Decimal)()), Function(callInfo)
                    callInfo(0) = 1
                    Return Nothing
                End Function).AndDoes(Function(callInfo)
                        callInfo(0) = 2
                        Return Nothing
                    End Function)
        End Sub

    End Class
End Namespace
