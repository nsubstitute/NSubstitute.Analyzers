using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NSubstitute.Analyzers.CSharp.CodeRefactoringProviders;
using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeRefactoringProvidersTests.IntroduceSubstituteRefactoringProviderTests;

public class IntroduceSubstituteCodeRefactoringActionsTests : CSharpCodeRefactoringProviderActionsVerifier, IIntroduceSubstituteCodeRefactoringActionsVerifier
{
    private static readonly string IntroduceReadonlySubstitutesTitle = "Introduce readonly substitutes for missing arguments";
    private static readonly string IntroduceLocalSubstitutesTitle = "Introduce local substitutes for missing arguments";
    private static readonly string IntroduceReadonlySubstituteForService = "Introduce readonly substitute for MyNamespace.IService";

    protected override CodeRefactoringProvider CodeRefactoringProvider { get; } = new IntroduceSubstituteCodeRefactoringProvider();

    [Fact]
    public async Task DoesNotCreateCodeActions_WhenConstructorHasNoParameters()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public class Foo
    {
        public Foo()
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var target = new Foo([||]);
        }
    }
}";

        await VerifyCodeActions(source, Array.Empty<string>());
    }

    [Fact]
    public async Task DoesNotCreateCodeActions_WhenMultipleCandidateConstructorsAvailable()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IService {}
    public interface IOtherService {}
    public class Foo
    {
        public Foo(IService service) { }
        public Foo(IOtherService otherService) { }
    }

    public class FooTests
    {
        public void Test()
        {
            var target = new Foo([||]);
        }
    }
}";

        await VerifyCodeActions(source, Array.Empty<string>());
    }

    [Fact]
    public async Task DoesNotCreateCodeActions_WhenAllParametersProvided()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IService {}
    public class Foo
    {
        public Foo(IService service) { }
    }

    public class FooTests
    {
        public void Test()
        {
            var service = Substitute.For<IService>();
            var target = new Foo([||]service);
        }
    }
}";
        await VerifyCodeActions(source, Array.Empty<string>());
    }

    [Fact]
    public async Task DoesNotCreateCodeActionsForIntroducingSpecificSubstitute_WhenArgumentAtPositionProvided()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IService {}
    public interface IOtherService {}
    public class Foo
    {
        public Foo(IService service, IOtherService otherService) { }
    }

    public class FooTests
    {
        public void Test()
        {
            var service = Substitute.For<IService>();
            var target = new Foo(service[||],);
        }
    }
}";
        await VerifyCodeActions(source, IntroduceLocalSubstitutesTitle, IntroduceReadonlySubstitutesTitle);
    }

    [Fact]
    public async Task DoesNotCreateCodeActionsForIntroducingLocalSubstitute_WhenLocalVariableCannotBeIntroduced()
    {
        const int refactoringCount = 3;
        var singleRefactoringActions = new[]
        {
            IntroduceReadonlySubstituteForService,
            IntroduceReadonlySubstitutesTitle
        };

        var allRefactoringsActions =
            Enumerable.Range(0, refactoringCount).SelectMany(_ => singleRefactoringActions).ToArray();

        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IService {}
    public interface IOtherService {}
    public class Foo
    {
        public Foo(IService service, IOtherService otherService) { }
    }

    public class FooTests
    {
        private readonly Foo foo = new Foo([||])
        public FooTests() => new Foo([||])
        public void Test() => new Foo([||])
    }
}";
        await VerifyCodeActions(source, allRefactoringsActions);
    }

    [Theory]
    [InlineData("new Foo([||],);", new[] { "Introduce local substitute for MyNamespace.IService", "Introduce readonly substitute for MyNamespace.IService" })]
    [InlineData("new Foo(,[||]);", new[] { "Introduce local substitute for MyNamespace.IOtherService", "Introduce readonly substitute for MyNamespace.IOtherService" })]
    public async Task CreatesCodeActionsForIntroducingSpecificSubstitute_WhenArgumentAtPositionNotProvided(string creationExpression, string[] expectedSpecificSubstituteActions)
    {
        var expectedAllActions = new[]
        {
            IntroduceLocalSubstitutesTitle,
            IntroduceReadonlySubstitutesTitle
        }.Concat(expectedSpecificSubstituteActions).ToArray();

        var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{}}
    public interface IOtherService {{}}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService) {{ }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var target = {creationExpression}
        }}
    }}
}}";
        await VerifyCodeActions(source, expectedAllActions);
    }

    [Fact]
    public async Task DoesNotCreateCodeActions_WhenSymbolIsNotConstructorInvocation()
    {
        var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IService {}
    public interface IOtherService {}
    public class Foo
    {
        public Foo([||]IService service, [||]IIOtherService otherService) { }
        public Baz(int x, [||]) { }
    }

    public class FooTests
    {
        public void Test()
        {
            var service = Substitute.For<IService>([||]);
            var x = FooBar(1, [||]);
            var z = new int[] { [||] };
        }

        private void FooBar([||]int x,[||]int y)
        {
        }
    }
}";
        await VerifyCodeActions(source);
    }
}