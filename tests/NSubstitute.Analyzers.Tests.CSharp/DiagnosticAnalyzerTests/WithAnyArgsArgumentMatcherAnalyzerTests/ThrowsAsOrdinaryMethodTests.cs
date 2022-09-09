using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ThrowsAsOrdinaryMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex: new Exception(), value: {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        object Bar(int? x);
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
        "ExceptionExtensions.Throws<Exception>({0})",
        "ExceptionExtensions.Throws<Exception>(value: {0})",
        "ExceptionExtensions.Throws({0}, new Exception())",
        "ExceptionExtensions.Throws(value: {0}, ex: new Exception())",
        "ExceptionExtensions.Throws(ex: new Exception(), value: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        object Bar(int? x);
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
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex: new Exception(), value: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        object Bar(int? x);
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
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex: new Exception(), value: {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        object this[int? x] {{ get; }}
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
        "ExceptionExtensions.Throws<Exception>({0})",
        "ExceptionExtensions.Throws<Exception>(value: {0})",
        "ExceptionExtensions.Throws({0}, new Exception())",
        "ExceptionExtensions.Throws(value: {0}, ex: new Exception())",
        "ExceptionExtensions.Throws(ex: new Exception(), value: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        object this[int? x] {{ get; }}
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
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>({0})",
        "ExceptionExtensions.ThrowsForAnyArgs<Exception>(value: {0})",
        "ExceptionExtensions.ThrowsForAnyArgs({0}, new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(value: {0}, ex: new Exception())",
        "ExceptionExtensions.ThrowsForAnyArgs(ex: new Exception(), value: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MyNamespace
{{
    public interface Foo
    {{
        object this[int? x] {{ get; }}
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