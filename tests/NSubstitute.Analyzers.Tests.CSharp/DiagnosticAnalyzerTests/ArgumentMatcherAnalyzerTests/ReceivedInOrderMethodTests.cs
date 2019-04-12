using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public class ReceivedInOrderMethodTests : ArgumentMatcherMisuseDiagnosticVerifier
    {
        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostic_WhenUsedWithinReceivedInOrderMethod(string arg)
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
            Received.InOrder(() => substitute.Bar({arg}) );
            Received.InOrder(() => {{ substitute.Bar({arg}); }} );
            Received.InOrder(delegate {{ substitute.Bar(Arg.Any<int>()); }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostic_WhenIndexer_UsedWithinReceivedInOrderMethod(string arg)
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
            Received.InOrder(() => {{ var x = substitute[{arg}]; }});
            Received.InOrder(delegate {{ var x = substitute[{arg}]; }});
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }
    }
}