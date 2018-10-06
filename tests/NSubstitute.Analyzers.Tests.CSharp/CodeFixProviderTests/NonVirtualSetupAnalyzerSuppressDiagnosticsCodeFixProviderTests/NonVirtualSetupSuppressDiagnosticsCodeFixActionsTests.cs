using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.NonVirtualSetupAnalyzerSuppressDiagnosticsCodeFixProviderTests
{
    public class NonVirtualSetupSuppressDiagnosticsCodeFixActionsTests : CSharpCodeFixActionsVerifier, INonVirtualSetupSuppressDiagnosticsCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForIndexer()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int this[int x] => 0;
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute[1].Returns(1);
        }
    }
}";
            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for indexer this[] in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForProperty()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar { get; }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar.Returns(1);
        }
    }
}";
            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for property Bar in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for namespace MyNamespace in nsubstitute.json"
            });
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForMethod()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar()
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar().Returns(1);
        }
    }
}";
            await VerifyCodeActions(source, new[]
            {
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for method Bar in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for class Foo in nsubstitute.json",
                $"Suppress {DiagnosticIdentifiers.NonVirtualSetupSpecification} for namespace MyNamespace in nsubstitute.json"
            });
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new NonVirtualSetupSuppressDiagnosticsCodeFixProvider();
        }
    }
}