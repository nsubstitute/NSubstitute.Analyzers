Imports NSubstitute.Core

Namespace Models
    Public Interface IFoo
        Sub VoidReturningMethod()
        
        Sub VoidReturningMethodWithArguments(ByVal a As Integer, ByVal b As Integer, ByVal c As Decimal)

        Function ObjectReturningMethod() As IFoo
        
        Function ObjectReturningMethodWithArguments(ByVal a As Integer, ByVal b As Integer, ByVal c As Decimal) As IFoo
        
        Function ObjectReturningMethodWithRefArguments(ByRef a As Integer, ByRef b As Integer, ByRef c As Decimal) As IFoo
        
        Function CallInfoReturningMethod() As ConfiguredCall
        
        ReadOnly Property ConfiguredCallReturningProperty As ConfiguredCall
        
        ReadOnly Property IntReturningProperty As Integer
        
        ReadOnly Property ObjectReturningProperty As IFoo
        
        Default ReadOnly Property Item(ByVal a As Integer) As Integer
        
        Default ReadOnly Property Item(ByVal a As Integer, ByVal b As Integer) As IFoo
        
        Default ReadOnly Property Item(ByVal a As Integer, ByVal b As Integer, ByVal c As Integer, ByVal d As Integer) As ConfiguredCall
        
    End Interface
End Namespace
