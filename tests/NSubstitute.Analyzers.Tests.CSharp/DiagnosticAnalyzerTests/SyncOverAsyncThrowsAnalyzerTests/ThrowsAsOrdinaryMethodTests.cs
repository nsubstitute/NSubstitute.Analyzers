using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SyncOverAsyncThrowsAnalyzerTests;

public class ThrowsAsOrdinaryMethodTests : SyncOverAsyncThrowsDiagnosticVerifier
{
    public override async Task ReportsDiagnostic_WhenUsedInTaskReturningMethod(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar();

        Task<object> FooBar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            [|ExceptionExtensions.{method}(substitute.Bar(), new Exception())|];
            [|ExceptionExtensions.{method}(value: substitute.Bar(), ex: new Exception())|];
            [|ExceptionExtensions.{method}(ex: new Exception(), value: substitute.Bar())|];
            [|ExceptionExtensions.{method}(substitute.FooBar(), new Exception())|];
            [|ExceptionExtensions.{method}(value: substitute.FooBar(), ex: new Exception())|];
            [|ExceptionExtensions.{method}(ex: new Exception(), value: substitute.FooBar())|];
        }}
    }}
}}";

        await VerifyDiagnostic(source, SyncOverAsyncThrowsDescriptor);
    }

    public override async Task ReportsDiagnostic_WhenUsedInTaskReturningProperty(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task Bar {{ get; set; }}

        Task<object> FooBar {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            [|ExceptionExtensions.{method}(substitute.Bar, new Exception())|];
            [|ExceptionExtensions.{method}(value: substitute.Bar, ex: new Exception())|];
            [|ExceptionExtensions.{method}(ex: new Exception(), value: substitute.Bar)|];
            [|ExceptionExtensions.{method}(substitute.FooBar, new Exception())|];
            [|ExceptionExtensions.{method}(value: substitute.FooBar, ex: new Exception())|];
            [|ExceptionExtensions.{method}(ex: new Exception(), value: substitute.FooBar)|];
        }}
    }}
}}";

        await VerifyDiagnostic(source, SyncOverAsyncThrowsDescriptor);
    }

    public override async Task ReportsDiagnostic_WhenUsedInTaskReturningIndexer(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        Task this[int x] {{ get; set; }}

        Task<object> this[int x, int y] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            [|ExceptionExtensions.{method}(substitute[0], new Exception())|];
            [|ExceptionExtensions.{method}(value: substitute[0], ex: new Exception())|];
            [|ExceptionExtensions.{method}(ex: new Exception(), value: substitute[0])|];
            [|ExceptionExtensions.{method}(substitute[0, 0], new Exception())|];
            [|ExceptionExtensions.{method}(value: substitute[0, 0], ex: new Exception())|];
            [|ExceptionExtensions.{method}(ex: new Exception(), value: substitute[0, 0])|];
        }}
    }}
}}";

        await VerifyDiagnostic(source, SyncOverAsyncThrowsDescriptor);
    }

    public override async Task ReportsNoDiagnostic_WhenUsedWithSyncMember(string method)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface IFoo
    {{
        object Bar();
        object FooBar {{ get; set; }}
        object this[int x] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            ExceptionExtensions.{method}(substitute.Bar(), new Exception());
            ExceptionExtensions.{method}(value: substitute.Bar(), ex: new Exception());
            ExceptionExtensions.{method}(ex: new Exception(), value: substitute.Bar());
            ExceptionExtensions.{method}(substitute.FooBar, new Exception());
            ExceptionExtensions.{method}(value: substitute.FooBar, ex: new Exception());
            ExceptionExtensions.{method}(ex: new Exception(), value: substitute.FooBar);
            ExceptionExtensions.{method}(substitute[0], new Exception());
            ExceptionExtensions.{method}(value: substitute[0], ex: new Exception());
            ExceptionExtensions.{method}(ex: new Exception(), value: substitute[0]);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}