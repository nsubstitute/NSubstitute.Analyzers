using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ConflictingRefOutAnalyzerTests
{
    [CombinatoryData("Returns")]
    public class ReturnsAsOrdinaryMethodTests : ConflictingRefOutDiagnosticVerifier
    {
        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;", "[|callInfo[1]|] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;", "[|callInfo[1]|] = 1;")]
        public async Task ReportsDiagnostic_When_AndDoesMethod_SetsSameArgument_AsPreviousSetupMethod(string method, string call, string previousCallArgAccess, string andDoesArgAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo => 1,
            callInfo =>
            {{
                {previousCallArgAccess}
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

            await VerifyDiagnostic(source, Descriptor);
        }

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;")]
        public async Task ReportsNoDiagnostics_When_AndDoesMethod_SetsDifferentArgument_AsPreviousSetupMethod(string method, string call, string andDoesArgAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo =>
            {{
                callInfo[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "var x = callInfo[1];")]
        [InlineData("substitute.Barr", "var x = callInfo[1];")]
        [InlineData("substitute[Arg.Any<int>()]", "var x = callInfo[1];")]
        public async Task ReportsNoDiagnostics_When_AndDoesMethod_AccessSameArguments_AsPreviousSetupMethod(string method, string call, string argAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(callInfo =>
            {{
                {argAccess}
                return 1;
            }}).AndDoes(callInfo =>
            {{
                {argAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryTheory]
        [InlineData]
        public async Task ReportsNoDiagnostics_When_AndDoesMethod_SetSameArguments_AsPreviousSetupMethod_SetsIndirectly(string method)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(Arg.Any<int>()).{method}(callInfo =>
            {{
                 callInfo.Args()[0] = 1;
                 callInfo.ArgTypes()[0] = typeof(int);
                 ((byte[])callInfo[0])[0] = 1;
                return 1;
            }}).AndDoes(callInfo =>
            {{
                callInfo[0] = 1;
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [CombinatoryTheory]
        [InlineData("substitute.Bar(Arg.Any<int>())", "callInfo[1] = 1;")]
        [InlineData("substitute.Barr", "callInfo[1] = 1;")]
        [InlineData("substitute[Arg.Any<int>()]", "callInfo[1] = 1;")]
        public async Task ReportsNoDiagnostic_When_AndDoesMethod_SetArgument_AndPreviousMethod_IsNotUsedWithCallInfo(string method, string call, string andDoesArgAccess)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public interface Foo
    {{
        int Bar(int x);

        int Barr {{ get; }}

        int this[int x] {{ get; }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = NSubstitute.Substitute.For<Foo>();
            {call}.{method}(1).AndDoes(callInfo =>
            {{
                {andDoesArgAccess}
            }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}