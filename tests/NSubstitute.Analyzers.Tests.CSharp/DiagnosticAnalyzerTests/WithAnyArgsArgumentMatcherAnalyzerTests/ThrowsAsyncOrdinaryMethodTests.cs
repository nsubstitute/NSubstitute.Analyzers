using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ThrowsAsyncOrdinaryMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex: new Exception(), value: {0})")]
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
            {string.Format(method, $"substitute.Bar({arg})")};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsync<Exception>({0})",
        "ExceptionExtensions.ThrowsAsync<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsAsync({0}, new Exception())",
        "ExceptionExtensions.ThrowsAsync(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsAsync(ex: new Exception(), value: {0})")]
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
            {string.Format(method, $"substitute.Bar({arg})")};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex: new Exception(), value: {0})")]
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
            {string.Format(method, $"substitute.Bar({arg})")};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex: new Exception(), value: {0})")]
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
            {string.Format(method, $"substitute[{arg}]")};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsync<Exception>({0})",
        "ExceptionExtensions.ThrowsAsync<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsAsync({0}, new Exception())",
        "ExceptionExtensions.ThrowsAsync(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsAsync(ex: new Exception(), value: {0})")]
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
            {string.Format(method, $"substitute[{arg}]")};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsAsyncForAnyArgs(ex: new Exception(), value: {0})")]
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
            {string.Format(method, $"substitute[{arg}]")};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}