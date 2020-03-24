using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NSubstitute.Analyzers.CSharp.CodeRefactoringProviders;
using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeRefactoringProvidersTests.IntroduceSubstituteRefactoringProviderTests
{
    public class IntroduceSubstituteCodeRefactoringProviderTests : CSharpCodeRefactoringProviderVerifier, IIntroduceSubstituteCodeRefactoringProviderVerifier
    {
        public static IEnumerable<object[]> AllArgumentsMissingTestCases
        {
            get
            {
                yield return new object[] { "new Foo([||]);", "new Foo(service, otherService);" };
                yield return new object[] { "new Foo([||])", "new Foo(service, otherService)" };
                yield return new object[] { "new Foo(, [||]);", "new Foo(service, otherService);" };
                yield return new object[] { "new Foo([||],);", "new Foo(service, otherService);" };
                yield return new object[] { "new Foo([||]", "new Foo(service, otherService" };
                yield return new object[] { "new Foo(, [||]", "new Foo(service, otherService" };
                yield return new object[] { "new Foo([||],", "new Foo(service, otherService" };
            }
        }

        public static IEnumerable<object[]> SomeArgumentMissingTestCases
        {
            get
            {
                yield return new object[]
                {
                    "new Foo([||],someOtherService, someAnotherService, );",
                    "new Foo(service, someOtherService, someAnotherService, yetAnotherService);"
                };
                yield return new object[]
                {
                    "new Foo([||],someOtherService, someAnotherService, )",
                    "new Foo(service, someOtherService, someAnotherService, yetAnotherService)"
                };
                yield return new object[]
                {
                    "new Foo(,someOtherService, someAnotherService, [||]);",
                    "new Foo(service, someOtherService, someAnotherService, yetAnotherService);"
                };
                yield return new object[]
                {
                    "new Foo([||],someOtherService, someAnotherService,",
                    "new Foo(service, someOtherService, someAnotherService, yetAnotherService"
                };
                yield return new object[]
                {
                    "new Foo(,someOtherService, someAnotherService, [||]",
                    "new Foo(service, someOtherService, someAnotherService, yetAnotherService"
                };
            }
        }

        public static IEnumerable<object[]> SpecificArgumentMissingTestCases
        {
            get
            {
                yield return new object[]
                {
                    "new Foo(service, [||],, someYetAnotherService);",
                    "new Foo(service, otherService,, someYetAnotherService);"
                };

                yield return new object[]
                {
                    "new Foo(service, [||],, someYetAnotherService)",
                    "new Foo(service, otherService,, someYetAnotherService)"
                };
            }
        }

        protected override CodeRefactoringProvider CodeRefactoringProvider { get; } = new IntroduceSubstituteCodeRefactoringProvider();

        [Theory]
        [MemberData(nameof(AllArgumentsMissingTestCases))]
        public async Task GeneratesConstructorArgumentListWithLocalVariables_WhenAllArgumentsMissing(string creationExpression, string expectedCreationExpression)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var target = {creationExpression}
        }}
    }}
}}";
            var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var service = Substitute.For<IService>();
            var otherService = Substitute.For<IOtherService>();
            var target = {expectedCreationExpression}
        }}
    }}
}}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 2);
        }

        [Fact]
        public async Task GeneratesConstructorArgumentListForAllArguments_WithFullyQualifiedName_WhenNamespaceNotImported()
        {
            var source = @"
namespace MyNamespace
{
    public interface IService { }
    public interface IOtherService { }
    public class Foo
    {
        public Foo(IService service, IOtherService otherService)
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
            var newSource = @"
namespace MyNamespace
{
    public interface IService { }
    public interface IOtherService { }
    public class Foo
    {
        public Foo(IService service, IOtherService otherService)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var service = NSubstitute.Substitute.For<IService>();
            var otherService = NSubstitute.Substitute.For<IOtherService>();
            var target = new Foo(service, otherService);
        }
    }
}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 2);
        }

        [Fact]
        public async Task GeneratesConstructorArgumentListForSpecificSubstitute_WithFullyQualifiedName_WhenNamespaceNotImported()
        {
            var source = @"
namespace MyNamespace
{
    public interface IService { }
    public interface IOtherService { }
    public class Foo
    {
        public Foo(IService service, IOtherService otherService)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var target = new Foo([||],);
        }
    }
}";
            var newSource = @"
namespace MyNamespace
{
    public interface IService { }
    public interface IOtherService { }
    public class Foo
    {
        public Foo(IService service, IOtherService otherService)
        {
        }
    }

    public class FooTests
    {
        public void Test()
        {
            var service = NSubstitute.Substitute.For<IService>();
            var target = new Foo(service,);
        }
    }
}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 0);
        }

        [Theory]
        [MemberData(nameof(SomeArgumentMissingTestCases))]
        public async Task GeneratesConstructorArgumentListWithLocalVariables_WhenSomeArgumentsMissing(string creationExpression, string expectedCreationExpression)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var someOtherService = Substitute.For<IOtherService>();
            var someAnotherService = Substitute.For<IAnotherService>();
            var target = {creationExpression}
        }}
    }}
}}";
            var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var service = Substitute.For<IService>();
            var yetAnotherService = Substitute.For<IYetAnotherService>();
            var someOtherService = Substitute.For<IOtherService>();
            var someAnotherService = Substitute.For<IAnotherService>();
            var target = {expectedCreationExpression}
        }}
    }}
}}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 2);
        }

        [Theory]
        [MemberData(nameof(AllArgumentsMissingTestCases))]
        public async Task GeneratesConstructorArgumentListWithReadonlyFields_WhenAllArgumentsMissing(string creationExpression, string expectedCreationExpression)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var target = {creationExpression}
        }}
    }}
}}";
            var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService)
        {{
        }}
    }}

    public class FooTests
    {{
        private readonly IService service = Substitute.For<IService>();
        private readonly IOtherService otherService = Substitute.For<IOtherService>();

        public void Test()
        {{
            var target = {expectedCreationExpression}
        }}
    }}
}}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 3);
        }

        [Theory]
        [MemberData(nameof(SomeArgumentMissingTestCases))]
        public async Task GeneratesConstructorArgumentListWithReadonlyFields_WhenSomeArgumentsMissing(string creationExpression, string expectedCreationExpression)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var someOtherService = Substitute.For<IOtherService>();
            var someAnotherService = Substitute.For<IAnotherService>();
            var target = {creationExpression}
        }}
    }}
}}";
            var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        private readonly IService service = Substitute.For<IService>();
        private readonly IYetAnotherService yetAnotherService = Substitute.For<IYetAnotherService>();

        public void Test()
        {{
            var someOtherService = Substitute.For<IOtherService>();
            var someAnotherService = Substitute.For<IAnotherService>();
            var target = {expectedCreationExpression}
        }}
    }}
}}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 3);
        }

        [Theory]
        [MemberData(nameof(SpecificArgumentMissingTestCases))]
        public async Task GeneratesConstructorArgumentListWithLocalVariable_ForSpecificArgument_WhenArgumentMissing(string creationExpression, string expectedCreationExpression)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var service = Substitute.For<IService>();
            var someYetAnotherService = Substitute.For<IYetAnotherService>();
            var target = {creationExpression}
        }}
    }}
}}";
            var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        public void Test()
        {{
            var otherService = Substitute.For<IOtherService>();
            var service = Substitute.For<IService>();
            var someYetAnotherService = Substitute.For<IYetAnotherService>();
            var target = {expectedCreationExpression}
        }}
    }}
}}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 0);
        }

        [Theory]
        [MemberData(nameof(SpecificArgumentMissingTestCases))]
        public async Task GeneratesConstructorArgumentListWithReadonlyFields_ForSpecificArgument_WhenArgumentMissing(string creationExpression, string expectedCreationExpression)
        {
            var source = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        private readonly IService service = Substitute.For<IService>();
        private readonly IYetAnotherService someYetAnotherService = Substitute.For<IYetAnotherService>();
        public void Test()
        {{
            var target = {creationExpression}
        }}
    }}
}}";
            var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public interface IService {{ }}
    public interface IOtherService {{ }}
    public interface IAnotherService {{ }}
    public interface IYetAnotherService {{ }}
    public class Foo
    {{
        public Foo(IService service, IOtherService otherService, IAnotherService anotherService, IYetAnotherService yetAnotherService)
        {{
        }}
    }}

    public class FooTests
    {{
        private readonly IOtherService otherService = Substitute.For<IOtherService>();
        private readonly IService service = Substitute.For<IService>();
        private readonly IYetAnotherService someYetAnotherService = Substitute.For<IYetAnotherService>();
        public void Test()
        {{
            var target = {expectedCreationExpression}
        }}
    }}
}}";
            await VerifyRefactoring(source, newSource, refactoringIndex: 1);
        }
    }
}