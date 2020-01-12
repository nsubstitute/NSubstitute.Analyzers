using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface IReEntrantSetupCodeFixProviderVerifier
    {
        Task ReplacesArgumentExpression_WithLambda(string arguments, string rewrittenArguments);

        Task ReplacesArgumentExpression_WithLambdaWithReducedTypes_WhenGeneratingArrayParamsArgument();
    }
}