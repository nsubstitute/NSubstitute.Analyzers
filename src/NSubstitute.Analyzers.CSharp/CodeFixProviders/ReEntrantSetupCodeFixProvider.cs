using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class ReEntrantSetupCodeFixProvider : AbstractReEntrantSetupCodeFixProvider<ArgumentListSyntax, ArgumentSyntax>
    {
        protected override int AwaitExpressionRawKind { get; } = (int)SyntaxKind.AwaitExpression;

        protected override ArgumentSyntax CreateUpdatedArgumentSyntaxNode(ArgumentSyntax argumentSyntaxNode)
        {
            return argumentSyntaxNode.WithExpression(CreateSimpleLambdaExpressionNode(argumentSyntaxNode.Expression));
        }

        protected override IEnumerable<SyntaxNode> GetParameterExpressionsFromArrayArgument(ArgumentSyntax argumentSyntaxNode)
        {
            return argumentSyntaxNode.Expression.GetParameterExpressionsFromArrayArgument()?.Select<ExpressionSyntax, SyntaxNode>(syntax => syntax);
        }

        protected override ArgumentSyntax CreateUpdatedParamsArgumentSyntaxNode(
            SyntaxGenerator syntaxGenerator,
            ITypeSymbol typeSymbol,
            ArgumentSyntax argumentSyntaxNode,
            IEnumerable<SyntaxNode> initializers)
        {
            var arrayCreationExpression = CreateArrayCreationExpression(syntaxGenerator, typeSymbol, initializers);

            return argumentSyntaxNode.WithExpression(arrayCreationExpression);
        }

        protected override IEnumerable<ArgumentSyntax> GetArguments(ArgumentListSyntax argumentSyntax) => argumentSyntax.Arguments;

        protected override SyntaxNode GetArgumentExpressionSyntax(ArgumentSyntax argumentSyntax) => argumentSyntax.Expression;

        private static SimpleLambdaExpressionSyntax CreateSimpleLambdaExpressionNode(SyntaxNode content)
        {
            return SimpleLambdaExpression(
                Parameter(Identifier("_").WithTrailingTrivia(Space)),
                (CSharpSyntaxNode)content.WithLeadingTrivia(Space));
        }

        private static ArrayCreationExpressionSyntax CreateArrayCreationExpression(
            SyntaxGenerator syntaxGenerator,
            ITypeSymbol typeSymbol,
            IEnumerable<SyntaxNode> initializers)
        {
            var arrayType = CreateArrayTypeNode(syntaxGenerator, typeSymbol);
            var syntaxes = CreateSimpleLambdaExpressions(initializers);

            var initializer = InitializerExpression(SyntaxKind.ArrayInitializerExpression, syntaxes);

            return ArrayCreationExpression(arrayType, initializer);
        }

        private static SeparatedSyntaxList<ExpressionSyntax> CreateSimpleLambdaExpressions(IEnumerable<SyntaxNode> initializers)
        {
            var expressions = initializers.Select<SyntaxNode, ExpressionSyntax>(CreateSimpleLambdaExpressionNode);
            return SeparatedList(expressions);
        }

        private static ArrayTypeSyntax CreateArrayTypeNode(SyntaxGenerator syntaxGenerator, ITypeSymbol type)
        {
            var typeSyntax = (TypeSyntax)syntaxGenerator.TypeExpression(type);

            var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())));

            return ArrayType(typeSyntax, arrayRankSpecifierSyntaxes).WithAdditionalAnnotations(Simplifier.Annotation);
        }
    }
}