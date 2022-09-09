using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public class ReturnsAsExtensionMethodTests : ForAnyArgsArgumentMatcherDiagnosticVerifier
{
    [CombinatoryData("ReturnsForAnyArgs(null)", "ReturnsNullForAnyArgs()")]
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
            substitute.Bar({arg}).{method};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("Returns(null)", "ReturnsNull()", "ReturnsForAll((object)null)")]
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
            substitute.Bar({arg}).{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ReturnsForAnyArgs(null)", "ReturnsNullForAnyArgs()")]
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
            substitute.Bar({arg}).{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ReturnsForAnyArgs(null)", "ReturnsNullForAnyArgs()")]
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
            substitute[{arg}].{method};
        }}
    }}
}}";
        await VerifyDiagnostic(source, WithAnyArgsArgumentMatcherUsage);
    }

    [CombinatoryData("Returns(null)", "ReturnsNull()", "ReturnsForAll((object)null)")]
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
            substitute[{arg}].{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData("ReturnsForAnyArgs(null)", "ReturnsNullForAnyArgs()")]
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
            substitute[{arg}].{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}