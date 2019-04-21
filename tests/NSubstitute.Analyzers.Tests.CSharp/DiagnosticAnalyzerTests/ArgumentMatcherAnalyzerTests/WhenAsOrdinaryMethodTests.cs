using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("SubstituteExtensions.When", "SubstituteExtensions.When<Foo>", "SubstituteExtensions.WhenForAnyArgs", "SubstituteExtensions.WhenForAnyArgs<Foo>")]
    public class WhenAsOrdinaryMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"using System;
using System.Threading.Tasks;
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
            {method}(substitute, delegate(Foo x) {{ x.Bar({arg}); }}).Do(x => throw new NullReferenceException());
            {method}(substitute, x => x.Bar({arg})).Do(x => throw new NullReferenceException());
            {method}(substitute, x => x.Bar({arg})).Do(x => {{ throw new NullReferenceException(); }});
            {method}(substitute, x => x.Bar({arg})).Do(x => {{ throw new NullReferenceException(); }});
            {method}(substitute, SubstituteCall).Do(x => {{ throw new NullReferenceException(); }});
        }}

        private Task SubstituteCall(Foo obj)
        {{
            obj.Bar(Arg.Any<int>());
            return Task.CompletedTask;
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
            {method}(substitute, delegate(Foo x) {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
            {method}(substitute, x => {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}