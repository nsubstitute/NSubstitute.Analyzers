using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface IPartialSubstituteUsedForUnsupportedTypeCodeFixActionsVerifier
{
    Task CreatesCorrectCodeFixActions_ForSubstituteForPartsOf();

    Task CreatesCorrectCodeFixActions_ForSubstituteFactoryCreatePartial();
}