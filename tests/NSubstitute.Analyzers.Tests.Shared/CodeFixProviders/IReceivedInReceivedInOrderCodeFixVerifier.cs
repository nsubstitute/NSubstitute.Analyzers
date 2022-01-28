using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface IReceivedInReceivedInOrderCodeFixVerifier
{
    Task RemovesReceivedChecks_WhenReceivedChecksHasNoArguments();

    Task RemovesReceivedChecks_WhenReceivedChecksHasArguments();
}