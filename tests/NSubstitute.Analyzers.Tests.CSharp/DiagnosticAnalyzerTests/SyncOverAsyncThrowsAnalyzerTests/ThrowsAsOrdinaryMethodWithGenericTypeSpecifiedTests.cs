using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SyncOverAsyncThrowsAnalyzerTests;

public class ThrowsAsOrdinaryMethodWithGenericTypeSpecifiedTests : SyncOverAsyncThrowsDiagnosticVerifier
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
            [|ExceptionExtensions.{method}<Exception>(substitute.Bar())|];
            [|ExceptionExtensions.{method}<Exception>(value: substitute.Bar())|];
            [|ExceptionExtensions.{method}<Exception>(substitute.FooBar())|];
            [|ExceptionExtensions.{method}<Exception>(value: substitute.FooBar())|];
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
            [|ExceptionExtensions.{method}<Exception>(substitute.Bar)|];
            [|ExceptionExtensions.{method}<Exception>(value: substitute.Bar)|];
            [|ExceptionExtensions.{method}<Exception>(substitute.FooBar)|];
            [|ExceptionExtensions.{method}<Exception>(value: substitute.FooBar)|];
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
            [|ExceptionExtensions.{method}<Exception>(substitute[0])|];
            [|ExceptionExtensions.{method}<Exception>(value: substitute[0])|];
            [|ExceptionExtensions.{method}<Exception>(substitute[0, 0])|];
            [|ExceptionExtensions.{method}<Exception>(value: substitute[0, 0])|];
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
            ExceptionExtensions.{method}<Exception>(substitute.Bar());
            ExceptionExtensions.{method}<Exception>(value: substitute.Bar());
            ExceptionExtensions.{method}<Exception>(substitute.FooBar);
            ExceptionExtensions.{method}<Exception>(value: substitute.FooBar);
            ExceptionExtensions.{method}<Exception>(substitute[0]);
            ExceptionExtensions.{method}<Exception>(value: substitute[0]);
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}