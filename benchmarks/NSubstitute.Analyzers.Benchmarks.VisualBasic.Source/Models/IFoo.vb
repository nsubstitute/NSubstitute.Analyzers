Imports NSubstitute.Core

Namespace Models
    Public Interface IFoo
        Sub VoidReturningMethod()
        Function ObjectReturningMethod() As IFoo
        Function CallInfoReturningMethod() As ConfiguredCall
        ReadOnly Property ConfiguredCallReturningProperty As ConfiguredCall
        Default ReadOnly Property Item(ByVal a As Integer, ByVal b As Integer, ByVal c As Integer, ByVal d As Integer) As ConfiguredCall
        Sub VoidReturningMethodWithArguments(ByVal x As Integer, ByVal y As Integer, ByVal z As Decimal)
        Function ObjectReturningMethodWithArguments(ByVal x As Integer, ByVal y As Integer, ByVal z As Decimal) As IFoo
        Function ObjectReturningMethodWithRefArguments(ByRef x As Integer, ByRef y As Integer, ByRef z As Decimal) As IFoo
        ReadOnly Property [Property] As Integer
        ReadOnly Property ObjectReturningProperty As IFoo
        Default ReadOnly Property Item(ByVal i As Integer) As Integer
        Default ReadOnly Property Item(ByVal i As Integer, ByVal y As Integer) As IFoo
    End Interface
End Namespace
