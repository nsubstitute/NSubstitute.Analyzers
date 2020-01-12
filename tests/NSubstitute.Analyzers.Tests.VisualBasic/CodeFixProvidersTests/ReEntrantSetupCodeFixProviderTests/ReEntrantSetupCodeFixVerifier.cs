using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReEntrantSetupCodeFixProviderTests
{
    public abstract class ReEntrantSetupCodeFixVerifier : VisualBasicCodeFixVerifier, IReEntrantSetupCodeFixProviderVerifier
    {
        [Theory]
        [InlineData("CreateReEntrantSubstitute(), CreateDefaultValue(), 1", "Function(x) CreateReEntrantSubstitute(), Function(x) CreateDefaultValue(), Function(x) 1")]
        [InlineData("CreateReEntrantSubstitute(), { CreateDefaultValue(), 1 }", "Function(x) CreateReEntrantSubstitute(), New System.Func(Of Core.CallInfo, Integer)() {Function(x) CreateDefaultValue(), Function(x) 1}")]
        [InlineData("CreateReEntrantSubstitute(), New Integer() {CreateDefaultValue(), 1}", "Function(x) CreateReEntrantSubstitute(), New System.Func(Of Core.CallInfo, Integer)() {Function(x) CreateDefaultValue(), Function(x) 1}")]
        [InlineData("returnThis:= CreateReEntrantSubstitute()", "returnThis:=Function(x) CreateReEntrantSubstitute()")]
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
    }
}