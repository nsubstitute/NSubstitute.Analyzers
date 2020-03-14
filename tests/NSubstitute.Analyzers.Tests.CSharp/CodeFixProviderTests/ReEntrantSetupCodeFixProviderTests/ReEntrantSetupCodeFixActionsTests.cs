using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReEntrantSetupCodeFixProviderTests
{
    public class ReEntrantSetupCodeFixActionsTests : CSharpCodeFixActionsVerifier, IReEntrantSetupCodeFixActionsVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new ReEntrantSetupAnalyzer();

        protected override CodeFixProvider CodeFixProvider { get; } = new ReEntrantSetupCodeFixProvider();

        [Theory]
        [InlineData("await CreateReEntrantSubstituteAsync(), await CreateDefaultValue()")]
        [InlineData("CreateReEntrantSubstitute(), await CreateDefaultValue()")]
        [InlineData("CreateReEntrantSubstitute(), new int[] { 1, await CreateDefaultValue() }")]
        public async Task DoesNotCreateCodeFixAction_WhenAnyArgumentSyntaxIsAsync(string arguments)
        {
            var source = $@"using System.Threading.Tasks;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public async Task Test()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns({arguments});
        }}

        private async Task<int> CreateReEntrantSubstituteAsync()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return await Task.FromResult(1);
        }}

        private int CreateReEntrantSubstitute()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }}

        private async Task<int> CreateDefaultValue()
        {{{{
            return await Task.FromResult(1);
        }}}}
    }} 
}}";
            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Theory]
        [InlineData("someArray")]
        [InlineData("Array.Empty<int>()")]
        public async Task DoesNotCreateCodeFixAction_WhenArrayParamsArgumentExpressionsCantBeRewritten(string paramsArgument)
        {
            var source = $@"using System;
using NSubstitute;

namespace MyNamespace
{{
    public class FooTests
    {{
        public interface IFoo
        {{
            int Id {{ get; }}
        }}
        
        public void Test()
        {{
            var substitute = Substitute.For<IFoo>();
            var someArray = new int[] {{ 1, 2, 3 }};
            substitute.Id.Returns(CreateReEntrantSubstitute(), {paramsArgument});
        }}

        private int CreateReEntrantSubstitute()
        {{
            var substitute = Substitute.For<IFoo>();
            substitute.Id.Returns(1);
            return 1;
        }}
    }} 
}}";
            await VerifyCodeActions(source, Array.Empty<string>());
        }
    }
}