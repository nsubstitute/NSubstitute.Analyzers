using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("ReturnsExtensions.ReturnsNull", "ReturnsExtensions.ReturnsNull<object>", "ReturnsExtensions.ReturnsNullForAnyArgs", "ReturnsExtensions.ReturnsNullForAnyArgs<object>")]
    public class ReturnsNullAsOrdinaryMethodTests : ArgumentMatcherDiagnosticVerifier
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
            var x = {method}(substitute.Bar({arg}));
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
            {method}(substitute[{arg}]);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}