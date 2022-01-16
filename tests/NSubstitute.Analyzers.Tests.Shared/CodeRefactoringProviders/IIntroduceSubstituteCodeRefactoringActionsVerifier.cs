using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;

public interface IIntroduceSubstituteCodeRefactoringActionsVerifier
{
    Task DoesNotCreateCodeActions_WhenConstructorHasNoParameters();

    Task DoesNotCreateCodeActions_WhenMultipleCandidateConstructorsAvailable();

    Task DoesNotCreateCodeActions_WhenAllParametersProvided();

    Task DoesNotCreateCodeActionsForIntroducingSpecificSubstitute_WhenArgumentAtPositionProvided();

    Task DoesNotCreateCodeActionsForIntroducingLocalSubstitute_WhenLocalVariableCannotBeIntroduced();

    Task CreatesCodeActionsForIntroducingSpecificSubstitute_WhenArgumentAtPositionNotProvided(string creationExpression, string[] expectedSpecificSubstituteActions);

    Task DoesNotCreateCodeActions_WhenSymbolIsNotConstructorInvocation();
}