using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface IReEntrantSetupCodeFixActionsVerifier
    {
        Task DoesNotCreateCodeFixAction_WhenAnyArgumentSyntaxIsAsync(string arguments);

        Task DoesNotCreateCodeFixAction_WhenArrayParamsArgumentExpressionsCantBeRewritten(string paramsArgument);
    }
}