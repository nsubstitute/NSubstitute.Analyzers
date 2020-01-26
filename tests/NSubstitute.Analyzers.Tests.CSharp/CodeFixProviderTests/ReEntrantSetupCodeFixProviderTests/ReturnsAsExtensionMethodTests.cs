using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReEntrantSetupCodeFixProviderTests
{
    public class ReturnsAsExtensionMethodTests : ReEntrantSetupCodeFixVerifier
    {
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
    }
}