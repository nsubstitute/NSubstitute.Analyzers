using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SyncOverAsyncThrowsAnalyzerTests;

public class ThrowsAsExtensionMethodTests : SyncOverAsyncThrowsDiagnosticVerifier
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
            [|substitute.Bar().{method}(new Exception())|];
            [|substitute.Bar().{method}(ex: new Exception())|];
            [|substitute.FooBar().{method}(new Exception())|];
            [|substitute.FooBar().{method}(ex: new Exception())|];
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
            [|substitute.Bar.{method}(new Exception())|];
            [|substitute.Bar.{method}(ex: new Exception())|];
            [|substitute.FooBar.{method}(new Exception())|];
            [|substitute.FooBar.{method}(ex: new Exception())|];
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
            [|substitute[0].{method}(new Exception())|];
            [|substitute[0].{method}(ex: new Exception())|];
            [|substitute[0, 0].{method}(new Exception())|];
            [|substitute[0, 0].{method}(ex: new Exception())|];
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
            substitute.Bar().{method}(new Exception());
            substitute.Bar().{method}(ex: new Exception());
            substitute.FooBar.{method}(new Exception());
            substitute.FooBar.{method}(ex: new Exception());
            substitute[0].{method}(new Exception());
            substitute[0].{method}(ex: new Exception());
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostic_WhenThrowsAsyncUsed(string method)
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

        Task Foo {{ get; set; }}
        Task<object> FooBarBar {{ get; set; }}

        Task this[int x] {{ get; set; }}
        Task<object> this[int x, int y] {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<IFoo>();
            substitute.Bar().{method}(new Exception());
            substitute.Bar().{method}(ex: new Exception());
            substitute.FooBar().{method}(new Exception());
            substitute.FooBar().{method}(ex: new Exception());

            substitute.Foo.{method}(new Exception());
            substitute.Foo.{method}(ex: new Exception());
            substitute.FooBarBar.{method}(new Exception());
            substitute.FooBarBar.{method}(ex: new Exception());

            substitute[0].{method}(new Exception());
            substitute[0].{method}(ex: new Exception());
            substitute[0, 0].{method}(new Exception());
            substitute[0, 0].{method}(ex: new Exception());
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}