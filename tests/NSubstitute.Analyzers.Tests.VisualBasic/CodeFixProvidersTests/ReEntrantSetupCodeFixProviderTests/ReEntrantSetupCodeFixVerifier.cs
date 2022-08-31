using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReEntrantSetupCodeFixProviderTests;

public abstract class ReEntrantSetupCodeFixVerifier : VisualBasicCodeFixVerifier, IReEntrantSetupCodeFixProviderVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new ReEntrantSetupAnalyzer();

    protected override CodeFixProvider CodeFixProvider { get; } = new ReEntrantSetupCodeFixProvider();

    [Theory]
    public abstract Task ReplacesArgumentExpression_WithLambda(string arguments, string rewrittenArguments);

    [Fact]
    public abstract Task ReplacesArgumentExpression_WithLambdaWithReducedTypes_WhenGeneratingArrayParamsArgument();

    [Fact]
    public abstract Task ReplacesArgumentExpression_WithLambdaWithNonGenericCallInfo_WhenGeneratingArrayParamsArgument();
}