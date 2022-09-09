using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ThrowsAsyncAsExtensionMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData("ThrowsAsyncForAnyArgs<Exception>()", "ThrowsAsyncForAnyArgs(new Exception())")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute.Bar({arg}).{method};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("ThrowsAsync<Exception>()", "ThrowsAsync(new Exception())")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute.Bar({arg}).{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ThrowsAsyncForAnyArgs<Exception>()", "ThrowsAsyncForAnyArgs(new Exception())")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute.Bar({arg}).{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ThrowsAsyncForAnyArgs<Exception>()", "ThrowsAsyncForAnyArgs(new Exception())")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task this[int? x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute[{arg}].{method};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("ThrowsAsync<Exception>()", "ThrowsAsync(new Exception())")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task this[int? x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute[{arg}].{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ThrowsAsyncForAnyArgs<Exception>()", "ThrowsAsyncForAnyArgs(new Exception())")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        Task this[int? x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>();
            substitute[{arg}].{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}