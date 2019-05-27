Imports NSubstitute.Core

Namespace Models
    Public Class Foo
        Implements IFoo

        Public Sub VoidReturningMethod() Implements IFoo.VoidReturningMethod
        End Sub

        Public Function ObjectReturningMethod() As IFoo Implements IFoo.ObjectReturningMethod
            Return Nothing
        End Function

        Public Function CallInfoReturningMethod() As ConfiguredCall Implements IFoo.CallInfoReturningMethod
            Return Nothing
        End Function

        Public ReadOnly Property ConfiguredCallReturningProperty As ConfiguredCall Implements IFoo.ConfiguredCallReturningProperty

        Default Public ReadOnly Property Item(a As Integer, b As Integer, c As Integer, d As Integer) As ConfiguredCall Implements IFoo.Item
            Get
                Return Nothing
            End Get
        End Property

        Public Sub VoidReturningMethodWithArguments(x As Integer, y As Integer, z As Decimal) Implements IFoo.VoidReturningMethodWithArguments
        End Sub

        Public Function ObjectReturningMethodWithArguments(x As Integer, y As Integer, z As Decimal) As IFoo Implements IFoo.ObjectReturningMethodWithArguments
            Return Nothing
        End Function

        Public Function ObjectReturningMethodWithRefArguments(ByRef x As Integer, ByRef y As Integer, ByRef z As Decimal) As IFoo Implements IFoo.ObjectReturningMethodWithRefArguments
            Return Nothing
        End Function

        Public ReadOnly Property [Property] As Integer Implements IFoo.[Property]
        Public ReadOnly Property ObjectReturningProperty As IFoo Implements IFoo.ObjectReturningProperty

        Default Public ReadOnly Property Item(i As Integer) As Integer Implements IFoo.Item
            Get
                Return Nothing
            End Get
        End Property

        Default Public ReadOnly Property Item(i As Integer, y As Integer) As IFoo Implements IFoo.Item
            Get
                Return Nothing
            End Get
        End Property
        
        Friend Overridable Function InternalObjectReturningMethod() As IFoo
            Return Nothing
        End Function

        Friend Overridable ReadOnly Property InternalObjectReturningProperty As IFoo
            Get
                Return Nothing
            End Get
        End Property

        Default Friend Overridable ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer, ByVal z As Integer) As IFoo
            Get
                Return Nothing
            End Get
        End Property
        
    End Class
End Namespace
