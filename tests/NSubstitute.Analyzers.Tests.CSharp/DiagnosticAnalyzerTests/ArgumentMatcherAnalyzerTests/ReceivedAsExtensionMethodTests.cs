using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public class ReceivedAsExtensionMethodTests : CSharpDiagnosticVerifier
    {
        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostics_WhenUsedWithReceivedCallsAssertionMethod(string arg)
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
            substitute.Received(1).Bar({arg});
        }}
    }}
}}";

            var secondSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public abstract class Foo3
    {{
        public abstract int Bar(int x);
    }}

    public class FooTests2
    {{
        public void Test3()
        {{
            var substitute = NSubstitute.Substitute.For<Foo3>();
            substitute.Received(1).Bar({arg});
        }}
    }}
}}";

            await VerifyDiagnostic(new[] { source, secondSource });
        }

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostics_WhenUsedWithReceivedCallsAssertionMethod_Indexer(string arg)
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
            var x = substitute.Received(1)[{arg}];
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}