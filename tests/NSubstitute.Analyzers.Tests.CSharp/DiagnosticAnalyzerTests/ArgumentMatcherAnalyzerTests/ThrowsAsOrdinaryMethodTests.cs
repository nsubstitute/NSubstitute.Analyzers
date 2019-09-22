using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    [CombinatoryData("ExceptionExtensions.Throws", "ExceptionExtensions.ThrowsForAnyArgs")]
    public class ThrowsAsOrdinaryMethodTests : ArgumentMatcherDiagnosticVerifier
    {
        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForMethodCall(string method, string arg)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            var x = {method}(substitute.Bar({arg}), new Exception());
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostics_WhenUsedWithSubstituteMethod_ForIndexerCall(string method, string arg)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            {method}(substitute[{arg}], new Exception());
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method, string arg)
        {
            var source = $@"using System;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
            {method}(substitute.Bar({arg}), new Exception());
        }}
    }}

    public static class ExceptionExtensions
    {{
        public static T Throws<T>(this object value, T ex) where T: Exception
        {{
            return default(T);
        }}

        public static T ThrowsForAnyArgs<T>(this object value, T ex) where T: Exception
        {{
            return default(T);
        }}
    }}
}}";

            await VerifyDiagnostic(source, ArgumentMatcherUsedWithoutSpecifyingCall);
        }
    }
}