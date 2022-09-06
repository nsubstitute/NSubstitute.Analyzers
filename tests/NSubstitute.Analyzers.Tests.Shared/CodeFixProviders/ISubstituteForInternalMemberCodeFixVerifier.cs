using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public interface ISubstituteForInternalMemberCodeFixVerifier
{
    Task AppendsInternalsVisibleTo_ToTopLevelCompilationUnit_WhenUsedWithInternalClass(int diagnosticIndex);

    Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass(int diagnosticIndex);

    Task AppendsInternalsVisibleTo_WhenUsedWithInternalClass_AndArgumentListNotEmpty(int diagnosticIndex);

    Task AppendsInternalsVisibleTo_WhenUsedWithNestedInternalClass(int diagnosticIndex);

    Task DoesNot_AppendsInternalsVisibleTo_WhenUsedWithPublicClass();

    Task DoesNot_AppendsInternalsVisibleTo_WhenInternalsVisibleToAppliedToDynamicProxyGenAssembly2();
}