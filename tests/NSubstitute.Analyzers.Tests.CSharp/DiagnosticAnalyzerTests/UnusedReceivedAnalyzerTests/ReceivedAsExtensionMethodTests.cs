using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests;

[CombinatoryData(
    "Received(Quantity.None())",
    "Received<Foo>(Quantity.None())",
    "Received()",
    "Received<Foo>()",
    "ReceivedWithAnyArgs(Quantity.None())",
    "ReceivedWithAnyArgs<Foo>(Quantity.None())",
    "ReceivedWithAnyArgs()",
    "ReceivedWithAnyArgs<Foo>()",
    "DidNotReceive()",
    "DidNotReceive<Foo>()",
    "DidNotReceiveWithAnyArgs()",
    "DidNotReceiveWithAnyArgs<Foo>()")]
public class ReceivedAsExtensionMethodTests : UnusedReceivedDiagnosticVerifier
{
    public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
    {
        var plainMethodName = method.Replace("<Foo>", string.Empty)
            .Replace("Quantity.None()", string.Empty)
            .Replace("()", string.Empty);

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
            [|substitute.{method}|];
        }}
    }}
}}";

        await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{plainMethodName}"". Correct: ""sub.{plainMethodName}().SomeCall();"". Incorrect: ""sub.{plainMethodName}();""");
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
            substitute.{method}.Bar();
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
            var bar = substitute.{method}.Bar;
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
            var bar = substitute.{method}[0];
        }}
    }}
}}";

        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received<Func<int>>(Quantity.None())",
        "Received()",
        "Received<Func<int>>()",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs<Func<int>>(Quantity.None())",
        "ReceivedWithAnyArgs()",
        "ReceivedWithAnyArgs<Func<int>>()",
        "DidNotReceive()",
        "DidNotReceive<Func<int>>()",
        "DidNotReceiveWithAnyArgs()",
        "DidNotReceiveWithAnyArgs<Func<int>>()")]
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
            substitute.{method}();
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    [CombinatoryData(
        "Received(Quantity.None())",
        "Received<Foo>(Quantity.None())",
        "Received(1, 1)",
        "Received<Foo>(1, 1)",
        "ReceivedWithAnyArgs(Quantity.None())",
        "ReceivedWithAnyArgs<Foo>(Quantity.None())",
        "ReceivedWithAnyArgs(1, 1)",
        "ReceivedWithAnyArgs<Foo>(1, 1)",
        "DidNotReceive(1, 1)",
        "DidNotReceive<Foo>(1, 1)",
        "DidNotReceiveWithAnyArgs(1, 1)",
        "DidNotReceiveWithAnyArgs<Foo>(1, 1)")]
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
            substitute.{method};
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }

    public override async Task ReportsNoDiagnostics_WhenSubscribingToEvent(string method)
    {
        var source = $@"using NSubstitute;
using NSubstitute.ReceivedExtensions;
using System;
namespace MyNamespace
{{
    public class Foo
    {{
        public event Action SomeEvent;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}.SomeEvent += Arg.Any<Action>();
        }}
    }}
}}";
        await VerifyNoDiagnostic(source);
    }
}