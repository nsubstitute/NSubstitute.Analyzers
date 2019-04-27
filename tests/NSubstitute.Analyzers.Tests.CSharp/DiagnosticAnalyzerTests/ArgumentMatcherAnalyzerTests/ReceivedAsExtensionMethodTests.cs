using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("Received", "Received<Foo>", "ReceivedWithAnyArgs", "ReceivedWithAnyArgs<Foo>", "DidNotReceive", "DidNotReceive<Foo>", "DidNotReceiveWithAnyArgs", "DidNotReceiveWithAnyArgs<Foo>")]
    public class ReceivedAsExtensionMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}().Bar({arg});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.{method}()[{arg}];
        }}
    }}
}}";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}(1m).Bar({arg});
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T Received<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}

        public static T ReceivedWithAnyArgs<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}

        public static T DidNotReceive<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}

        public static T DidNotReceiveWithAnyArgs<T>(this T returnValue, decimal x)
        {{
            return default(T);
        }}
    }}
}}";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }
    }
}