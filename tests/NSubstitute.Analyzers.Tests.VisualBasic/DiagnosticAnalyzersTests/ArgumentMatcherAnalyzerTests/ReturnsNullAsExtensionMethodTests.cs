using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("ReturnsNull", "ReturnsNullForAnyArgs")]
    public class ReturnsNullAsExtensionMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract Foo Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar({arg}).{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract Foo this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[{arg}].{method}();
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
        {
            var source = $@"using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract Foo Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            var x = substitute.Bar({arg}).{method}();
        }}
    }}

    public static class ReturnsExtensions
    {{
        public static T ReturnsNull<T>(this T returnValue)
        {{
            return default(T);
        }}

        public static T ReturnsNullForAnyArgs<T>(this T returnValue)
        {{
            return default(T);
        }}
    }}

}}";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }
    }
}