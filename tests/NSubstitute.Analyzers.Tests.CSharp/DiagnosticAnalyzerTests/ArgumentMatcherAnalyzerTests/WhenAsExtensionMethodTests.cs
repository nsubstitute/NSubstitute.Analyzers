using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("When", "When<Foo>", "WhenForAnyArgs", "WhenForAnyArgs<Foo>")]
    public class WhenAsExtensionMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"using System;
using NSubstitute;

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
            substitute.{method}(delegate(Foo x) {{ x.Bar({arg}); }}).Do(x => throw new NullReferenceException());
            substitute.{method}(x => x.Bar({arg})).Do(x => throw new NullReferenceException());
            substitute.{method}(x => x.Bar({arg})).Do(x => {{ throw new NullReferenceException(); }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"using NSubstitute;
using System;

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
            substitute.{method}(delegate(Foo x) {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
            substitute.{method}(x => {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}