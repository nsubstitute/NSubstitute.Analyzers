using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders
{
    public interface IIntroduceSubstituteCodeRefactoringProviderVerifier
    {
        Task GeneratesConstructorArgumentListWithLocalVariables_WhenAllArgumentsMissing(string creationExpression, string expectedCreationExpression);

        Task GeneratesConstructorArgumentListForAllArguments_WithFullyQualifiedName_WhenNamespaceNotImported();

        Task GeneratesConstructorArgumentListForSpecificSubstitute_WithFullyQualifiedName_WhenNamespaceNotImported();

        Task GeneratesConstructorArgumentListWithLocalVariables_WhenSomeArgumentsMissing(string creationExpression, string expectedCreationExpression);

        Task GeneratesConstructorArgumentListWithReadonlyFields_WhenAllArgumentsMissing(string creationExpression, string expectedCreationExpression);

        Task GeneratesConstructorArgumentListWithReadonlyFields_WhenSomeArgumentsMissing(string creationExpression, string expectedCreationExpression);

        Task GeneratesConstructorArgumentListWithLocalVariable_ForSpecificArgument_WhenArgumentMissing(string creationExpression, string expectedCreationExpression);

        Task GeneratesConstructorArgumentListWithReadonlyFields_ForSpecificArgument_WhenArgumentMissing(string creationExpression, string expectedCreationExpression);
    }
}