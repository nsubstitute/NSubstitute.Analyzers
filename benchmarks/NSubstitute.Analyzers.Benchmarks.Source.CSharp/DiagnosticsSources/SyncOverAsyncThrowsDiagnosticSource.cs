using System;
using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;
using NSubstitute.ExceptionExtensions;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class SyncOverAsyncThrowsDiagnosticSource
    {
        public void NS5003_SyncThrowsUsedInAsyncMethod()
        {
            var substitute = Substitute.For<Foo>();
            substitute.TaskReturningAsyncMethod().Throws(new Exception());
            substitute.GenericTaskReturningAsyncMethod().Throws(new Exception());
            substitute.TaskReturningAsyncMethod().Throws<Exception>();
            substitute.GenericTaskReturningAsyncMethod().Throws<Exception>();

            ExceptionExtensions.ExceptionExtensions.Throws(substitute.TaskReturningAsyncMethod(), new Exception());
            ExceptionExtensions.ExceptionExtensions.Throws(substitute.GenericTaskReturningAsyncMethod(), new Exception());
            ExceptionExtensions.ExceptionExtensions.Throws<Exception>(substitute.TaskReturningAsyncMethod());
            ExceptionExtensions.ExceptionExtensions.Throws<Exception>(substitute.GenericTaskReturningAsyncMethod());
        }
    }
}