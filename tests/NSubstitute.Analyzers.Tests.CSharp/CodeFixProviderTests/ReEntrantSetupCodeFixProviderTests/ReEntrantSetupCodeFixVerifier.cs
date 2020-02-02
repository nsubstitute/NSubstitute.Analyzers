using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReEntrantSetupCodeFixProviderTests
{
    public abstract class ReEntrantSetupCodeFixVerifier : CSharpCodeFixVerifier, IReEntrantSetupCodeFixProviderVerifier
    {
        [Theory]
        [InlineData("CreateReEntrantSubstitute(), CreateDefaultValue(), 1", "_ => CreateReEntrantSubstitute(), _ => CreateDefaultValue(), _ => 1")]
        [InlineData("CreateReEntrantSubstitute(), new [] { CreateDefaultValue(), 1 }", "_ => CreateReEntrantSubstitute(), new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
        [InlineData("CreateReEntrantSubstitute(), new int[] { CreateDefaultValue(), 1 }", "_ => CreateReEntrantSubstitute(), new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
        [InlineData("returnThis: CreateReEntrantSubstitute()", "returnThis: _ => CreateReEntrantSubstitute()")]
        [InlineData("returnThis: CreateReEntrantSubstitute(), returnThese: new [] { CreateDefaultValue(), 1 }", "returnThis: _ => CreateReEntrantSubstitute(), returnThese: new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
        [InlineData("returnThis: CreateReEntrantSubstitute(), returnThese: new int[] { CreateDefaultValue(), 1 }", "returnThis: _ => CreateReEntrantSubstitute(), returnThese: new System.Func<NSubstitute.Core.CallInfo, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
        public abstract Task ReplacesArgumentExpression_WithLambda(string arguments, string rewrittenArguments);

        [Fact]
        public abstract Task ReplacesArgumentExpression_WithLambdaWithReducedTypes_WhenGeneratingArrayParamsArgument();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new ReEntrantSetupCodeFixProvider();
        }

        protected override CompilationOptions GetCompilationOptions()
        {
            return new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, warningLevel: 1);
        }
    }
}