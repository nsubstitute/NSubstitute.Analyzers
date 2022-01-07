using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface IInternalSetupSpecificationCodeFixProviderVerifier
{
    Task ChangesInternalToPublic_ForIndexer_WhenUsedWithInternalMember(string method);

    Task ChangesInternalToPublic_ForProperty_WhenUsedWithInternalMember(string method);

    Task ChangesInternalToPublic_ForMethod_WhenUsedWithInternalMember(string method);

    Task AppendsProtectedInternal_ToIndexer_WhenUsedWithInternalMember(string method);

    Task AppendsProtectedInternal_ToProperty_WhenUsedWithInternalMember(string method);

    Task AppendsProtectedInternal_ToMethod_WhenUsedWithInternalMember(string method);

    Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalMember(string method, string call);

    Task AppendsInternalsVisibleToWithFullyQualifiedName_WhenUsedWithInternalMemberAndCompilerServicesNotImported(string method);
}