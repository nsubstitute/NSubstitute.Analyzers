using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests;

[CombinatoryData(
    "ReceivedExtensions.Received(substitute, Quantity.None())",
    "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
    "SubstituteExtensions.Received(substitute)",
    "SubstituteExtensions.Received<Foo>(substitute)",
    "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
    "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
    "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
    "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute)",
    "SubstituteExtensions.DidNotReceive(substitute)",
    "SubstituteExtensions.DidNotReceive<Foo>(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
    "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute)")]
public class ReceivedAsOrdinaryMethodTests : UnusedReceivedDiagnosticVerifier
{
    public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
    {
        var plainMethodName = method.Replace("<Foo>", string.Empty)
            .Replace("(substitute, Quantity.None())", string.Empty)
            .Replace("(substitute)", string.Empty);

        var planMethodNameWithoutNamespace = plainMethodName.Replace("SubstituteExtensions.", string.Empty)
            .Replace("ReceivedExtensions.", string.Empty);

        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
namespace MyNamespace
{{
    public interface Foo
    {{
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            [|{method}|];
        }}
    }}
}}";

        await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{planMethodNameWithoutNamespace}"". Correct: ""{plainMethodName}(sub).SomeCall();"". Incorrect: ""{plainMethodName}(sub);""");
    }

    public override async Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class FooBar
    {{
    }}

    public interface Foo
    {{
        FooBar Bar();
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}.Bar();
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{

    public interface Foo
    {{
        int Bar {{ get; set; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var bar = {method}.Bar;
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
namespace MyNamespace
{{

    public interface Foo
    {{
        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var bar = {method}[0];
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Func<int>>(substitute, Quantity.None())",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received<Func<int>>(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Func<int>>(substitute, Quantity.None())",
        "SubstituteExtensions.ReceivedWithAnyArgs<Func<int>>(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive<Func<int>>(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<int>>(substitute)")]
    public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method)
    {
        var source = $@"using System;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Func<int>>();
            {method}();
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
        "SubstituteExtensions.Received(substitute, 1, 1)",
        "SubstituteExtensions.Received<Foo>(substitute, 1, 1)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute, 1, 1)")]
    public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
    {
        var source = $@"using System;

namespace NSubstitute
{{
    public class Quantity
    {{
        public static Quantity None() => null;
    }}

    public class Foo
    {{
    }}

    public static class SubstituteExtensions
    {{
        public static T Received<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, int x, int y)
        {{
            return default(T);
        }}
    }}
    
    public static class ReceivedExtensions
    {{
        public static T Received<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, Quantity x)
        {{
            return default(T);
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo substitute = null;
            {method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}