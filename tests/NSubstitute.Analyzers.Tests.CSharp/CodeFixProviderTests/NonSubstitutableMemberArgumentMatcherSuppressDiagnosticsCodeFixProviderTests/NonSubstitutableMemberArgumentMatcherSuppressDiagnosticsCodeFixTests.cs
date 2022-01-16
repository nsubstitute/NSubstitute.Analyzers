using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProviderTests;

public class NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixTests
    : CSharpSuppressDiagnosticSettingsVerifier, INonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberArgumentMatcherAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider();

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenArgMatcherUsedInNonVirtualMethod()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public int Bar(int arg)
        {
            return 2;
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<Foo>();
            substitute.Bar(Arg.Any<int>());
        }
    }
}";

        await VerifySuppressionSettings(source, "M:MyNamespace.Foo.Bar(System.Int32)~System.Int32", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettings_WhenArgMatcherUsedInNonVirtualIndexer()
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
            _ = substitute[Arg.Any<int>()];
        }
    }
}";

        await VerifySuppressionSettings(source, "P:MyNamespace.Foo.Item(System.Int32)", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettingsForClass_WhenSettingsValueForNonVirtualMember_AndSelectingClassSuppression()
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
            _ = substitute[Arg.Any<int>()];
        }
    }
}";

        await VerifySuppressionSettings(source, "T:MyNamespace.Foo", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage, codeFixIndex: 1);
    }

    [Fact]
    public async Task SuppressesDiagnosticsInSettingsForNamespace_WhenSettingsValueForNonVirtualMember_AndSelectingNamespaceSuppression()
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
            _ = substitute[Arg.Any<int>()];
        }
    }
}";

        await VerifySuppressionSettings(source, "N:MyNamespace", DiagnosticIdentifiers.NonSubstitutableMemberArgumentMatcherUsage, codeFixIndex: 2);
    }
}