using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public interface ISubstituteForInternalMemberCodeFixVerifier
    {
        Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass();

        Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass();

        Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty();

        Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass();

        Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass();
    }
}