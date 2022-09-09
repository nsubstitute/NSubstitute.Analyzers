Imports System
Imports NSubstitute.Core
Imports System.Threading.Tasks

Namespace Models
    Public Class Foo
        Implements IFoo

        Public Sub VoidReturningMethod() Implements IFoo.VoidReturningMethod
        End Sub

        Public Sub VoidReturningMethodWithArguments(a As Integer, b As Integer, c As Decimal) Implements IFoo.VoidReturningMethodWithArguments
        End Sub
        
        Public Function TaskReturningAsyncMethod() As Task
            Return Task.CompletedTask
        End Function

        Public Function GenericTaskReturningAsyncMethod() As Task(Of Object)
            Return Task.FromResult(New Object())
        End Function
        
        Public Function ObjectReturningMethod() As IFoo Implements IFoo.ObjectReturningMethod
            Return Nothing
        End Function
        
        Public Function ObjectReturningMethodWithArguments(a As Integer, b As Integer, c As Decimal) As IFoo Implements IFoo.ObjectReturningMethodWithArguments
            Return Nothing
        End Function

        Public Function ObjectReturningMethodWithArguments(a As Action(Of Integer), b As Integer, c As Decimal) As IFoo
            Return Nothing
        End Function
        
        Public Function ObjectReturningMethodWithRefArguments(ByRef a As Integer, ByRef b As Integer, ByRef c As Decimal) As IFoo Implements IFoo.ObjectReturningMethodWithRefArguments
            Return Nothing
        End Function
        
        Friend Overridable Function InternalObjectReturningMethod() As IFoo
            Return Nothing
        End Function

        Friend Overridable Function InternalObjectReturningMethodWithArguments(ByVal a As Integer) As IFoo
            Return Nothing
        End Function
        
        Friend Overridable ReadOnly Property InternalObjectReturningProperty As IFoo
            Get
                Return Nothing
            End Get
        End Property
        
        Public Function CallInfoReturningMethod() As ConfiguredCall Implements IFoo.CallInfoReturningMethod
            Return Nothing
        End Function

        Public ReadOnly Property ConfiguredCallReturningProperty As ConfiguredCall Implements IFoo.ConfiguredCallReturningProperty

        Default Public ReadOnly Property Item(a As Integer, b As Integer, c As Integer, d As Integer) As ConfiguredCall Implements IFoo.Item
            Get
                Return Nothing
            End Get
        End Property
        
        Public Property IntReturningProperty As Integer Implements IFoo.IntReturningProperty
        Public ReadOnly Property ObjectReturningProperty As IFoo Implements IFoo.ObjectReturningProperty

        Default Public ReadOnly Property Item(a As Integer) As Integer Implements IFoo.Item
            Get
                Return Nothing
            End Get
        End Property

        Default Public ReadOnly Property Item(a As Integer, b As Integer) As IFoo Implements IFoo.Item
            Get
                Return Nothing
            End Get
        End Property
        
        Default Friend Overridable ReadOnly Property Item(ByVal a As Integer, ByVal b As Integer, ByVal c As Integer) As IFoo
            Get
                Return Nothing
            End Get
        End Property
        
    End Class
End Namespace
