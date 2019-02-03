using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    [CombinatoryData("Received", "Received<Foo>", "ReceivedWithAnyArgs", "ReceivedWithAnyArgs<Foo>", "DidNotReceive", "DidNotReceive<Foo>", "DidNotReceiveWithAnyArgs", "DidNotReceiveWithAnyArgs<Foo>")]
    public class ReceivedAsExtensionMethodTests : UnusedReceivedDiagnosticVerifier
    {
        public override async Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method)
        {
            var plainMethodName = method.Replace("<Foo>", string.Empty);
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
            [|substitute.{method}()|];
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor, $@"Unused received check. To fix, make sure there is a call after ""{plainMethodName}"". Correct: ""sub.{plainMethodName}().SomeCall();"". Incorrect: ""sub.{plainMethodName}();""");
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
            substitute.{method}().Bar();
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
            var bar = substitute.{method}().Bar;
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
            var bar = substitute.{method}()[0];
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryData("Received", "Received<Func<int>>", "ReceivedWithAnyArgs", "ReceivedWithAnyArgs<Func<int>>", "DidNotReceive", "DidNotReceive<Func<int>>", "DidNotReceiveWithAnyArgs", "DidNotReceiveWithAnyArgs<Func<int>>")]
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
            substitute.{method}()();
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
            substitute.{method}(1m);
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }
    }
}