using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SyntaxGeneratorExtensions
    {
        public static SyntaxNode SubstituteForInvocationExpression(
            this SyntaxGenerator syntaxGenerator,
            IParameterSymbol parameterSymbol)
        {
            var qualifiedName = syntaxGenerator.DottedName("NSubstitute.Substitute");

            var genericName = syntaxGenerator.GenericName("For", syntaxGenerator.TypeExpression(parameterSymbol.Type));

            var memberAccessExpression = syntaxGenerator.MemberAccessExpression(qualifiedName, genericName);

            var invocationExpression = syntaxGenerator.InvocationExpression(memberAccessExpression);

            return invocationExpression;
        }

        public static SyntaxNode CallInfoCallbackTypeSyntax(
            this SyntaxGenerator syntaxGenerator,
            ITypeSymbol returnedType)
        {
            var typeSyntax = syntaxGenerator.TypeExpression(returnedType);
            var genericName = syntaxGenerator.GenericName("Func", syntaxGenerator.DottedName("NSubstitute.Core.CallInfo"), typeSyntax);
            var qualifiedNameSyntax = syntaxGenerator.QualifiedName(syntaxGenerator.IdentifierName("System"), genericName);

            return qualifiedNameSyntax.WithAdditionalAnnotations(Simplifier.Annotation);
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
}