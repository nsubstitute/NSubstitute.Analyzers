using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface IConstructorArgumentsForInterfaceCodeFixVerifier
    {
        Task RemovesInvocationArguments_WhenGenericFor_UsedWithParametersForInterface();

        Task RemovesInvocationArguments_WhenNonGenericFor_UsedWithParametersForInterface();

        Task RemovesInvocationArguments_WhenSubstituteFactoryCreate_UsedWithParametersForInterface();
    }
}