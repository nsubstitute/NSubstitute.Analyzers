using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ReEntrantSetupCodeFixProviderTests;

public abstract class ReEntrantSetupCodeFixVerifier : CSharpCodeFixVerifier, IReEntrantSetupCodeFixProviderVerifier
{
    protected ReEntrantSetupCodeFixVerifier()
        : base(CSharpWorkspaceFactory.Default.WithWarningLevel(1))
    {
    }

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new ReEntrantSetupAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new ReEntrantSetupCodeFixProvider();

    [Theory]
    [InlineData("CreateReEntrantSubstitute(), CreateDefaultValue(), 1", "_ => CreateReEntrantSubstitute(), _ => CreateDefaultValue(), _ => 1")]
    [InlineData("CreateReEntrantSubstitute(), new [] { CreateDefaultValue(), 1 }", "_ => CreateReEntrantSubstitute(), new System.Func<NSubstitute.Core.CallInfo<int>, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    [InlineData("CreateReEntrantSubstitute(), new int[] { CreateDefaultValue(), 1 }", "_ => CreateReEntrantSubstitute(), new System.Func<NSubstitute.Core.CallInfo<int>, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    [InlineData("returnThis: CreateReEntrantSubstitute()", "returnThis: _ => CreateReEntrantSubstitute()")]
    [InlineData("returnThis: CreateReEntrantSubstitute(), returnThese: new [] { CreateDefaultValue(), 1 }", "returnThis: _ => CreateReEntrantSubstitute(), returnThese: new System.Func<NSubstitute.Core.CallInfo<int>, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    [InlineData("returnThis: CreateReEntrantSubstitute(), returnThese: new int[] { CreateDefaultValue(), 1 }", "returnThis: _ => CreateReEntrantSubstitute(), returnThese: new System.Func<NSubstitute.Core.CallInfo<int>, int>[] { _ => CreateDefaultValue(), _ => 1 }")]
    public abstract Task ReplacesArgumentExpression_WithLambda(string arguments, string rewrittenArguments);

    [Fact]
    public abstract Task ReplacesArgumentExpression_WithLambdaWithReducedTypes_WhenGeneratingArrayParamsArgument();

    [Fact]
    public abstract Task ReplacesArgumentExpression_WithLambdaWithNonGenericCallInfo_WhenGeneratingArrayParamsArgument();
}