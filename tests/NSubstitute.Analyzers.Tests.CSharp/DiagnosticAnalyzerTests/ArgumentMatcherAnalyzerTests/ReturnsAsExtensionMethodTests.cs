using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public class ReturnsAsExtensionMethodTests : ArgumentMatcherMisuseDiagnosticVerifier
    {
        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostics_WhenUsedWithSetupMethod(string arg)
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
            var x = substitute.Bar({arg}).Returns(1);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [Theory]
        [InlineData("[|Arg.Any<int>()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        public async Task ReportsDiagnostics_WhenUsedWithoutSetupMethod(string arg)
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
            substitute.Bar({arg});
        }}
    }}
}}";
            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostics_WhenUsedWitSetupMethod_Indexer(string arg)
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
            substitute[{arg}].Returns(1);
        }}
    }}
}}";

            await VerifyNoDiagnostic(source);
        }

        [Theory]
        [InlineData("[|Arg.Any<int>()|]")]
        [InlineData("[|Arg.Is(1)|]")]
        public async Task ReportsDiagnostics_WhenUsedWithoutSetupMethod_Indexer(string arg)
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
            var x = substitute[{arg}];
        }}
    }}
}}";

            await VerifyDiagnostic(source, ArgumentMatcherUsedOutsideOfCallDescriptor);
        }
    }
}