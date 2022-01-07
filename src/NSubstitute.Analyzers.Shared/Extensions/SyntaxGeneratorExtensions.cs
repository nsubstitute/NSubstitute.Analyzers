using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class SyntaxGeneratorExtensions
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

    public static SyntaxNode InternalVisibleToDynamicProxyAttributeList(this SyntaxGenerator syntaxGenerator)
    {
        var attributeArguments =
            syntaxGenerator.AttributeArgument(syntaxGenerator.LiteralExpression("DynamicProxyGenAssembly2"));

        return syntaxGenerator.Attribute(
            "System.Runtime.CompilerServices.InternalsVisibleTo",
            attributeArguments);
    }
}