using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class ReEntrantSetupCodeFixProvider : AbstractReEntrantSetupCodeFixProvider<ArgumentListSyntax, ArgumentSyntax, TypeSyntax>
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

        protected override ArgumentSyntax CreateUpdatedParamsArgumentSyntaxNode(ArgumentSyntax argumentSyntaxNode, TypeSyntax typeSyntax)
        {
            var expression = argumentSyntaxNode.Expression;
            ArrayCreationExpressionSyntax resultArrayCreationExpressionSyntax;

            switch (expression)
            {
                case ArrayCreationExpressionSyntax arrayCreationExpressionSyntax:
                    resultArrayCreationExpressionSyntax = CreateArrayCreationExpression(
                        arrayCreationExpressionSyntax.Initializer,
                        typeSyntax);
                    break;
                case ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax:
                    resultArrayCreationExpressionSyntax = CreateArrayCreationExpression(
                        implicitArrayCreationExpressionSyntax.Initializer,
                        typeSyntax);
                    break;
                default:
                    throw new ArgumentException($"{argumentSyntaxNode.Kind()} is not recognized as array initialization", nameof(argumentSyntaxNode));
            }

            return argumentSyntaxNode.WithExpression(resultArrayCreationExpressionSyntax);
        }

        protected override IEnumerable<ArgumentSyntax> GetArguments(ArgumentListSyntax argumentSyntax)
        {
            return argumentSyntax.Arguments;
        }

        protected override SyntaxNode GetArgumentExpressionSyntax(ArgumentSyntax argumentSyntax)
        {
            return argumentSyntax.Expression;
        }

        private static SimpleLambdaExpressionSyntax CreateSimpleLambdaExpressionNode(SyntaxNode content)
        {
            return SimpleLambdaExpression(
                Parameter(Identifier("_").WithTrailingTrivia(Space)),
                (CSharpSyntaxNode)content.WithLeadingTrivia(Space));
        }

        private static ArrayCreationExpressionSyntax CreateArrayCreationExpression(
            InitializerExpressionSyntax initializerExpressionSyntax,
            TypeSyntax typeSyntax)
        {
            var arrayType = CreateArrayTypeNode(typeSyntax);
            var syntaxes = CreateSimpleLambdaExpressions(initializerExpressionSyntax);

            var initializer = InitializerExpression(SyntaxKind.ArrayInitializerExpression, SeparatedList(syntaxes));

            return ArrayCreationExpression(arrayType, initializer);
        }

        private static IEnumerable<ExpressionSyntax> CreateSimpleLambdaExpressions(InitializerExpressionSyntax initializerExpressionSyntax)
        {
            return initializerExpressionSyntax.Expressions.Select(CreateSimpleLambdaExpressionNode);
        }

        private static ArrayTypeSyntax CreateArrayTypeNode(TypeSyntax typeSyntax)
        {
            var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())));

            return ArrayType(typeSyntax, arrayRankSpecifierSyntaxes).WithAdditionalAnnotations(Simplifier.Annotation);
        }
    }
}