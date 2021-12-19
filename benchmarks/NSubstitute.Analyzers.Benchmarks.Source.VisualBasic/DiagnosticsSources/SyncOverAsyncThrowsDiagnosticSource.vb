Imports System
Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models
Imports NSubstitute.ExceptionExtensions

Namespace DiagnosticsSources
    Public Class SyncOverAsyncThrowsDiagnosticSource
        Public Sub NS5003_SyncThrowsUsedInAsyncMethod()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            substitute.TaskReturningAsyncMethod().Throws(New Exception())
            substitute.GenericTaskReturningAsyncMethod().Throws(New Exception())
            substitute.TaskReturningAsyncMethod().Throws(Of Exception)()
            substitute.GenericTaskReturningAsyncMethod().Throws(Of Exception)()
            
            ExceptionExtensions.ExceptionExtensions.Throws(substitute.TaskReturningAsyncMethod(), New Exception())
            ExceptionExtensions.ExceptionExtensions.Throws(substitute.GenericTaskReturningAsyncMethod(), New Exception())
            ExceptionExtensions.ExceptionExtensions.Throws(Of Exception)(substitute.TaskReturningAsyncMethod())
            ExceptionExtensions.ExceptionExtensions.Throws(Of Exception)(substitute.GenericTaskReturningAsyncMethod())
        End Sub
    End Class
End Namespace