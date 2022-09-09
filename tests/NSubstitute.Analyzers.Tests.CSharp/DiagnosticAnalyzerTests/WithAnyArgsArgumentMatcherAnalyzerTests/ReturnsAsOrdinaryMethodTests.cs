using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReturnsAsOrdinaryMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData(
        "SubstituteExtensions.ReturnsForAnyArgs({0}, null)",
        "SubstituteExtensions.ReturnsForAnyArgs(value: {0}, returnThis: null)",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis: null, value: {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value: {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

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
        "SubstituteExtensions.Returns({0}, null)",
        "SubstituteExtensions.Returns(value: {0}, returnThis: null)",
        "SubstituteExtensions.Returns(returnThis: null, value: {0})",
        "ReturnsExtensions.ReturnsNull({0})",
        "ReturnsExtensions.ReturnsNull(value: {0})",
        "ReturnsForAllExtensions.ReturnsForAll({0}, (object)null)",
        "ReturnsForAllExtensions.ReturnsForAll(substitute: {0}, returnThis: (object)null)",
        "ReturnsForAllExtensions.ReturnsForAll(returnThis: (object)null, substitute: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NSubstitute.Extensions;

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
        "SubstituteExtensions.ReturnsForAnyArgs({0}, null)",
        "SubstituteExtensions.ReturnsForAnyArgs(value: {0}, returnThis: null)",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis: null, value: {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

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
        "SubstituteExtensions.ReturnsForAnyArgs({0}, null)",
        "SubstituteExtensions.ReturnsForAnyArgs(value: {0}, returnThis: null)",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis: null, value: {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value: {0})")]
    public override async Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

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
        "SubstituteExtensions.Returns({0}, null)",
        "SubstituteExtensions.Returns(value: {0}, returnThis: null)",
        "SubstituteExtensions.Returns(returnThis: null, value: {0})",
        "ReturnsExtensions.ReturnsNull({0})",
        "ReturnsExtensions.ReturnsNull(value: {0})",
        "ReturnsForAllExtensions.ReturnsForAll({0}, (object)null)",
        "ReturnsForAllExtensions.ReturnsForAll(substitute: {0}, returnThis: (object)null)",
        "ReturnsForAllExtensions.ReturnsForAll(returnThis: (object)null, substitute: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NSubstitute.Extensions;

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
        "SubstituteExtensions.ReturnsForAnyArgs({0}, null)",
        "SubstituteExtensions.ReturnsForAnyArgs(value: {0}, returnThis: null)",
        "SubstituteExtensions.ReturnsForAnyArgs(returnThis: null, value: {0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs({0})",
        "ReturnsExtensions.ReturnsNullForAnyArgs(value: {0})")]
    public override async Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NSubstitute.Extensions;

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