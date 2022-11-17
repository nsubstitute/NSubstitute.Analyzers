using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReEntrantSetupCodeFixProviderTests;

public class ReturnsAsExtensionMethodTests : ReEntrantSetupCodeFixVerifier
{
    [Theory]
    [InlineData("CreateReEntrantSubstitute(), CreateDefaultValue(), 1", "_ => CreateReEntrantSubstitute(), _ => CreateDefaultValue(), _ => 1")]
    [InlineData("CreateDefaultValue(), 1, CreateReEntrantSubstitute()", "_ => CreateDefaultValue(), _ => 1, _ => CreateReEntrantSubstitute()")]
    [InlineData("CreateReEntrantSubstitute(), new [] { CreateDefaultValue(), 1 }", "_ => CreateReEntrantSubstitute(), new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    [InlineData("CreateReEntrantSubstitute(), new int[] { CreateDefaultValue(), 1 }", "_ => CreateReEntrantSubstitute(), new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    [InlineData("CreateDefaultValue(), new int[] { 1, CreateReEntrantSubstitute() }", "_ => CreateDefaultValue(), new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => 1, _ => CreateReEntrantSubstitute() }")]
    [InlineData("returnThis: CreateReEntrantSubstitute()", "returnThis: _ => CreateReEntrantSubstitute()")]
    [InlineData("returnThis: CreateReEntrantSubstitute(), returnThese: new [] { CreateDefaultValue(), 1 }", "returnThis: _ => CreateReEntrantSubstitute(), returnThese: new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    [InlineData("returnThis: CreateReEntrantSubstitute(), returnThese: new int[] { CreateDefaultValue(), 1 }", "returnThis: _ => CreateReEntrantSubstitute(), returnThese: new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    public override async Task ReplacesArgumentExpression_WithLambda(string arguments, string rewrittenArguments)
    {
        var oldSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public FooTests()
        {{
            firstSubstitute.Id.Returns(45);
        }}
        
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public void Test()
        {{
            var secondSubstitute = Substitute.For<IFoo>();
            secondSubstitute.Id.Returns({arguments});
        }}

        private int CreateReEntrantSubstitute()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }}

        private int CreateDefaultValue()
        {{
            return 1;
        }}
    }} 
}}";

        var newSource = $@"using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public FooTests()
        {{
            firstSubstitute.Id.Returns(45);
        }}
        
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public void Test()
        {{
            var secondSubstitute = Substitute.For<IFoo>();
            secondSubstitute.Id.Returns({rewrittenArguments});
        }}

        private int CreateReEntrantSubstitute()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }}

        private int CreateDefaultValue()
        {{
            return 1;
        }}
    }} 
}}";
        await VerifyFix(oldSource, newSource);
    }

    [Fact]
    public override async Task ReplacesArgumentExpression_WithLambdaWithReducedTypes_WhenGeneratingArrayParamsArgument()
    {
        var oldSource = @"using NSubstitute;
using NSubstitute.Core;
using System;

namespace MyNamespace
{
    public class FooTests
    {
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public static int Value { get; set; }

        public FooTests()
        {
            firstSubstitute.Id.Returns(45);
        }
        
        public interface IFoo
        {
            int Id { get; }
        }
        
        public void Test()
        {
            var secondSubstitute = Substitute.For<IFoo>();
            secondSubstitute.Id.Returns(CreateReEntrantSubstitute(), new[] { MyNamespace.FooTests.Value });
        }

        private int CreateReEntrantSubstitute()
        {
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }
    }
}";

        var newSource = @"using NSubstitute;
using NSubstitute.Core;
using System;

namespace MyNamespace
{
    public class FooTests
    {
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public static int Value { get; set; }

        public FooTests()
        {
            firstSubstitute.Id.Returns(45);
        }
        
        public interface IFoo
        {
            int Id { get; }
        }
        
        public void Test()
        {
            var secondSubstitute = Substitute.For<IFoo>();
            secondSubstitute.Id.Returns(_ => CreateReEntrantSubstitute(), new Func<CallInfo, int>[] { _ => MyNamespace.FooTests.Value });
        }

        private int CreateReEntrantSubstitute()
        {
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }
    }
}";
        await VerifyFix(oldSource, newSource);
    }

    [Fact]
    public override async Task ReplacesArgumentExpression_WithLambdaWithNonGenericCallInfo_WhenGeneratingArrayParamsArgument()
    {
        var oldSource = @"using NSubstitute;
using NSubstitute.Core;
using System;

namespace MyNamespace
{
    public class FooTests
    {
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public static int Value { get; set; }

        public FooTests()
        {
            firstSubstitute.Id.Returns(45);
        }
        
        public interface IFoo
        {
            int Id { get; }
        }
        
        public void Test()
        {
            var secondSubstitute = Substitute.For<IFoo>();
            secondSubstitute.Id.Returns(CreateReEntrantSubstitute(), new[] { MyNamespace.FooTests.Value });
        }

        private int CreateReEntrantSubstitute()
        {
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }
    }
}";

        var newSource = @"using NSubstitute;
using NSubstitute.Core;
using System;

namespace MyNamespace
{
    public class FooTests
    {
        private IFoo firstSubstitute = Substitute.For<IFoo>();

        public static int Value { get; set; }

        public FooTests()
        {
            firstSubstitute.Id.Returns(45);
        }
        
        public interface IFoo
        {
            int Id { get; }
        }
        
        public void Test()
        {
            var secondSubstitute = Substitute.For<IFoo>();
            secondSubstitute.Id.Returns(_ => CreateReEntrantSubstitute(), new Func<CallInfo, int>[] { _ => MyNamespace.FooTests.Value });
        }

        private int CreateReEntrantSubstitute()
        {
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }
    }
}";
        await VerifyFix(oldSource, newSource, version: NSubstituteVersion.NSubstitute4_2_2);
    }
}