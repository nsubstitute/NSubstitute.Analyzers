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
using System.Threading.Tasks;
using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int Bar(int? x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}(delegate(Foo x) {{ x.Bar({arg}); }}).Do(x => throw new NullReferenceException());
            substitute.{method}(x => x.Bar({arg})).Do(x => throw new NullReferenceException());
            substitute.{method}(SubstituteCall).Do(x => {{ throw new NullReferenceException(); }});
        }}

        private Task SubstituteCall(Foo obj)
        {{
            obj.Bar({arg});
            return Task.CompletedTask;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"using System;
using System.Threading.Tasks;
using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo
    {{
        public abstract int this[int? x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.{method}(delegate(Foo x) {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
            substitute.{method}(x => {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
            substitute.{method}(SubstituteCall).Do(x => {{ throw new NullReferenceException(); }});
        }}

        private Task SubstituteCall(Foo obj)
        {{
            _ = obj[{arg}];
            return Task.CompletedTask;
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
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
            substitute.{method}(delegate(Foo x) {{ x.Bar({arg}); }}, 1);
            substitute.{method}(x => x.Bar({arg}), 1);
        }}

        private Task SubstituteCall(Foo obj)
        {{
            obj.Bar({arg});
            return Task.CompletedTask;
        }}
    }}

    public static class SubstituteExtensions
    {{
        public static T When<T>(this T substitute, System.Action<T> substituteCall, int x)
        {{
            return default(T);
        }}

        public static T WhenForAnyArgs<T>(this T substitute, System.Action<T> substituteCall, int x)
        {{
            return default(T);
        }}
    }}
}}";
            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }
    }
}