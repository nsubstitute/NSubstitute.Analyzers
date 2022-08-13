using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SyncOverAsyncThrowsAnalyzerTests;

public class ThrowsAsExtensionMethodWithGenericTypeSpecifiedTests : SyncOverAsyncThrowsDiagnosticVerifier
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
            [|substitute.Bar().{method}<Exception>()|];
            [|substitute.FooBar().{method}<Exception>()|];
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
            [|substitute.Bar.{method}<Exception>()|];
            [|substitute.FooBar.{method}<Exception>()|];
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
            [|substitute[0].{method}<Exception>()|];
            [|substitute[0, 0].{method}<Exception>()|];
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
            substitute.Bar().{method}<Exception>();
            substitute.FooBar.{method}<Exception>();
            substitute[0].{method}<Exception>();
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
            substitute.Bar().{method}<Exception>();
            substitute.Bar().{method}<Exception>();
            substitute.FooBar().{method}<Exception>();
            substitute.FooBar().{method}<Exception>();

            substitute.Foo.{method}<Exception>();
            substitute.Foo.{method}<Exception>();
            substitute.FooBarBar.{method}<Exception>();
            substitute.FooBarBar.{method}<Exception>();

            substitute[0].{method}<Exception>();
            substitute[0].{method}<Exception>();
            substitute[0, 0].{method}<Exception>();
            substitute[0, 0].{method}<Exception>();
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }
}