using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    [CombinatoryData(
        "ReceivedExtensions.Received(substitute, Quantity.None())",
        "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
        "ReceivedExtensions.Received<Foo>(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.Received<Foo>(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.Received(substitute)",
        "SubstituteExtensions.Received(substitute: substitute)",
        "SubstituteExtensions.Received<Foo>(substitute)",
        "SubstituteExtensions.Received<Foo>(substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, requiredQuantity: Quantity.None())",
        "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(requiredQuantity: Quantity.None(), substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute)",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute)",
        "SubstituteExtensions.DidNotReceive(substitute)",
        "SubstituteExtensions.DidNotReceive(substitute: substitute)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute)",
        "SubstituteExtensions.DidNotReceive<Foo>(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute)",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute: substitute)")]
    public class ReceivedAsOrdinaryMethodTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
        {
            var plainMethodName = method.Replace("<Foo>", string.Empty)
                .Replace("(substitute, Quantity.None())", string.Empty)
                .Replace("(substitute)", string.Empty)
                .Replace("(substitute: substitute)", string.Empty)
                .Replace("(substitute: substitute, requiredQuantity: Quantity.None())", string.Empty)
                .Replace("(requiredQuantity: Quantity.None(), substitute: substitute)", string.Empty);

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
            "ReceivedExtensions.Received(substitute: substitute, requiredQuantity: Quantity.None())",
            "ReceivedExtensions.Received(requiredQuantity: Quantity.None(), substitute: substitute)",
            "ReceivedExtensions.Received<Func<int>>(substitute, Quantity.None())",
            "ReceivedExtensions.Received<Func<int>>(substitute: substitute, requiredQuantity: Quantity.None())",
            "ReceivedExtensions.Received<Func<int>>(requiredQuantity: Quantity.None(), substitute: substitute)",
            "SubstituteExtensions.Received(substitute)",
            "SubstituteExtensions.Received(substitute: substitute)",
            "SubstituteExtensions.Received<Func<int>>(substitute)",
            "SubstituteExtensions.Received<Func<int>>(substitute: substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, requiredQuantity: Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(requiredQuantity: Quantity.None(), substitute: substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs<Func<int>>(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs<Func<int>>(substitute: substitute, requiredQuantity: Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs<Func<int>>(requiredQuantity: Quantity.None(), substitute: substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Func<int>>(substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Func<int>>(substitute: substitute)",
            "SubstituteExtensions.DidNotReceive(substitute)",
            "SubstituteExtensions.DidNotReceive(substitute: substitute)",
            "SubstituteExtensions.DidNotReceive<Func<int>>(substitute)",
            "SubstituteExtensions.DidNotReceive<Func<int>>(substitute: substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<int>>(substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<int>>(substitute: substitute)")]
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
            "ReceivedExtensions.Received(substitute: substitute, x: Quantity.None())",
            "ReceivedExtensions.Received(x: Quantity.None(), substitute: substitute)",
            "ReceivedExtensions.Received<Foo>(substitute, Quantity.None())",
            "ReceivedExtensions.Received<Foo>(substitute: substitute, x: Quantity.None())",
            "ReceivedExtensions.Received<Foo>(x: Quantity.None(), substitute: substitute)",
            "SubstituteExtensions.Received(substitute, 1, 1)",
            "SubstituteExtensions.Received(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.Received(x: 1, y: 1, substitute: substitute)",
            "SubstituteExtensions.Received<Foo>(substitute, 1, 1)",
            "SubstituteExtensions.Received<Foo>(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.Received<Foo>(x: 1, y: 1, substitute: substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(substitute: substitute, x: Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs(x: Quantity.None(), substitute: substitute)",
            "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute, Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, x: Quantity.None())",
            "ReceivedExtensions.ReceivedWithAnyArgs<Foo>(x: Quantity.None(), substitute: substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute, 1, 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs(x: 1, y: 1, substitute: substitute)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute, 1, 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.ReceivedWithAnyArgs<Foo>(x: 1, y: 1, substitute: substitute)",
            "SubstituteExtensions.DidNotReceive(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceive(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.DidNotReceive(x: 1, y: 1, substitute: substitute)",
            "SubstituteExtensions.DidNotReceive<Foo>(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceive<Foo>(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.DidNotReceive<Foo>(x: 1, y: 1, substitute: substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs(x: 1, y: 1, substitute: substitute)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute, 1, 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(substitute: substitute, x: 1, y: 1)",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>(x: 1, y: 1, substitute: substitute)")]
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
}