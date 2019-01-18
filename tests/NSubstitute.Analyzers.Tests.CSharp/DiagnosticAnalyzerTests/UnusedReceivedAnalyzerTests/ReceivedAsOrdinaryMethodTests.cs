using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    [CombinatoryData(
        "SubstituteExtensions.Received",
        "SubstituteExtensions.Received<Foo>",
        "SubstituteExtensions.ReceivedWithAnyArgs",
        "SubstituteExtensions.ReceivedWithAnyArgs<Foo>",
        "SubstituteExtensions.DidNotReceive",
        "SubstituteExtensions.DidNotReceive<Foo>",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs",
        "SubstituteExtensions.DidNotReceiveWithAnyArgs<Foo>")]
    public class ReceivedAsOrdinaryMethodTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
        {
            var plainMethodName =  method.Replace("<Foo>", string.Empty);
            var planMethodNameWithoutNamespace = plainMethodName.Replace("SubstituteExtensions.", string.Empty);

            var source = $@"using NSubstitute;

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
            [|{method}(substitute)|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{planMethodNameWithoutNamespace}"". Correct: ""{plainMethodName}(sub).SomeCall();"". Incorrect: ""{plainMethodName}(sub);""");
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess(string method)
        {
            var source = $@"using NSubstitute;

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
            {method}(substitute).Bar();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess(string method)
        {
            var source = $@"using NSubstitute;

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
            var bar = {method}(substitute).Bar;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess(string method)
        {
            var source = $@"using NSubstitute;

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
            var bar = {method}(substitute)[0];
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData(
            "SubstituteExtensions.Received",
            "SubstituteExtensions.Received<Func<int>>",
            "SubstituteExtensions.ReceivedWithAnyArgs",
            "SubstituteExtensions.ReceivedWithAnyArgs<Func<int>>",
            "SubstituteExtensions.DidNotReceive",
            "SubstituteExtensions.DidNotReceive<Func<int>>",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs",
            "SubstituteExtensions.DidNotReceiveWithAnyArgs<Func<int>>")]
        public override async Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Func<int>>();
            {method}(substitute)();
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method)
        {
            var source = $@"using System;

namespace NSubstitute
{{
    public class Foo
    {{
    }}

    public static class SubstituteExtensions
    {{
        public static T Received<T>(this T substitute, decimal x) where T : class
        {{
            return null;
        }}

        public static T ReceivedWithAnyArgs<T>(this T substitute, decimal x) where T : class
        {{
            return null;
        }}

        public static T DidNotReceive<T>(this T substitute, decimal x) where T : class
        {{
            return null;
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T substitute, decimal x) where T : class
        {{
            return null;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {method}(substitute, 1m);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }
    }
}