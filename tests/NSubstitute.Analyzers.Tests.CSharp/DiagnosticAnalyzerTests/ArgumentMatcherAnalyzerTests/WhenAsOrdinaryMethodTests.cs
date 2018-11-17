using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public class WhenAsOrdinaryMethodTests : CSharpDiagnosticVerifier
    {
        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostics_WhenUsedWithSetupMethod(string arg)
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
            SubstituteExtensions.When(substitute, delegate(Foo x) {{ x.Bar({arg}); }}).Do(x => throw new NullReferenceException());
            SubstituteExtensions.When(substitute, x => x.Bar({arg})).Do(x => throw new NullReferenceException());
            SubstituteExtensions.When(substitute, x => x.Bar({arg})).Do(x => {{ throw new NullReferenceException(); }});
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ArgumentMatcherUsedOutsideOfCall,
                Severity = DiagnosticSeverity.Warning,
                Message = "Arg matcher used outside of context",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 28)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
        public async Task ReportsNoDiagnostics_WhenUsedWitSetupMethod_Indexer(string arg)
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
            SubstituteExtensions.When(substitute, delegate(Foo x) {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
            SubstituteExtensions.When(substitute, x => {{ var y = x[{arg}]; }}).Do(x => throw new NullReferenceException());
        }}
    }}
}}";

            await VerifyDiagnostic(source);
        }

        [Theory]
        [InlineData("Arg.Any<int>()")]
        [InlineData("Arg.Is(1)")]
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

            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.ArgumentMatcherUsedOutsideOfCall,
                Severity = DiagnosticSeverity.Warning,
                Message = "Arg matcher used outside of context",
                Locations = new[]
                {
                    new DiagnosticResultLocation(15, 32)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }
    }
}