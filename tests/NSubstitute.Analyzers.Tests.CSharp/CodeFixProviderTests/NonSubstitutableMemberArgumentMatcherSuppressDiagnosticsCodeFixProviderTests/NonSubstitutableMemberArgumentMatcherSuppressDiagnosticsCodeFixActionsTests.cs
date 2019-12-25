using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProviderTests
{
    public class NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixActionsTests : CSharpCodeFixActionsVerifier, INonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForIndexer()
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public int this[int? firstArg] => 2;
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var substitute = Substitute.For<Foo>(); 
            _ = substitute[Arg.Any<int>()];
        }}
    }}
}}";
            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for indexer this[] in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForMethod()
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class Foo
    {{
        public static int Bar(int? firstArg)
        {{
            return 2;
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            Foo.Bar(Arg.Any<int>());
        }}
    }}
}}";
            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for method Bar in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage} for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task DoesNotCreateCodeFixActions_WhenArgMatchesIsUsedInStandaloneExpression()
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public void Test()
        {{
            Arg.Any<int>();
        }}
    }}
}}";
            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Fact]
        public async Task DoesNotCreateCodeFixActions_WhenArgMatchesIsUsedInConstructor()
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public FooTests(int firstArg)
        {{
        }}

        public void Test()
        {{
            new FooTests(Arg.Any<int>());
        }}
    }}
}}";
            await VerifyCodeActions(source, Array.Empty<string>());
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonSubstitutableMemberArgumentMatcherAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider();
        }
    }
}