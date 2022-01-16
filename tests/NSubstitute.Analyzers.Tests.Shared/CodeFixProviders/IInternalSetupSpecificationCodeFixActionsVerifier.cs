using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface IInternalSetupSpecificationCodeFixActionsVerifier
{
    Task CreateCodeActions_InProperOrder();

    Task DoesNotCreateCodeActions_WhenSymbol_DoesNotBelongToCompilation();
}