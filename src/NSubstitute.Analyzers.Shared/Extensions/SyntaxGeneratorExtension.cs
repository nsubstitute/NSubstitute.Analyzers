using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SyntaxGeneratorExtension
    {
        public static SyntaxNode SubstituteForInvocationExpression(
            this SyntaxGenerator syntaxGenerator,
            IParameterSymbol parameterSymbol)
        {
            var qualifiedName = syntaxGenerator.QualifiedName(
                syntaxGenerator.IdentifierName("NSubstitute"),
                syntaxGenerator.IdentifierName("Substitute"));

            var genericName = syntaxGenerator.GenericName("For", syntaxGenerator.TypeExpression(parameterSymbol.Type));

            var memberAccessExpression = syntaxGenerator.MemberAccessExpression(qualifiedName, genericName);

            var invocationExpression = syntaxGenerator.InvocationExpression(memberAccessExpression);

            return invocationExpression;
        }
    }
}