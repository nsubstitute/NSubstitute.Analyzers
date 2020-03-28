using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.Extensions;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal sealed class ReEntrantSetupCodeFixProvider : AbstractReEntrantSetupCodeFixProvider<ArgumentListSyntax, ArgumentSyntax, TypeSyntax>
    {
        protected override ArgumentSyntax CreateUpdatedArgumentSyntaxNode(ArgumentSyntax argumentSyntaxNode)
        {
            var expressionSyntax = argumentSyntaxNode.GetExpression();

            var lambdaExpression = CreateSingleLineLambdaExpression(expressionSyntax);

            switch (argumentSyntaxNode)
            {
                case SimpleArgumentSyntax simpleArgumentSyntax:
                    return simpleArgumentSyntax.WithExpression(lambdaExpression);
            }

            return argumentSyntaxNode;
        }

        protected override ArgumentSyntax CreateUpdatedParamsArgumentSyntaxNode(ArgumentSyntax argumentSyntaxNode, TypeSyntax typeSyntax)
        {
            if (!(argumentSyntaxNode is SimpleArgumentSyntax simpleArgumentSyntax))
            {
                return argumentSyntaxNode;
            }

            var expression = argumentSyntaxNode.GetExpression();
            ArrayCreationExpressionSyntax resultArrayCreationExpressionSyntax;

            switch (expression)
            {
                case ArrayCreationExpressionSyntax arrayCreationExpressionSyntax:
                    resultArrayCreationExpressionSyntax = CreateArrayCreationExpression(
                        arrayCreationExpressionSyntax.Initializer.Initializers,
                        typeSyntax);
                    break;
                case CollectionInitializerSyntax implicitArrayCreationExpressionSyntax:
                    resultArrayCreationExpressionSyntax = CreateArrayCreationExpression(
                        implicitArrayCreationExpressionSyntax.Initializers,
                        typeSyntax);
                    break;
                default:
                    throw new ArgumentException($"{argumentSyntaxNode.Kind()} is not recognized as array initialization", nameof(argumentSyntaxNode));
            }

            return simpleArgumentSyntax.WithExpression(resultArrayCreationExpressionSyntax);
        }

        protected override SyntaxNode GetArgumentExpressionSyntax(ArgumentSyntax argumentSyntax)
        {
            return argumentSyntax.GetExpression();
        }

        protected override IEnumerable<SyntaxNode> GetParameterExpressionsFromArrayArgument(ArgumentSyntax argumentSyntaxNode)
        {
            return argumentSyntaxNode.GetExpression().GetParameterExpressionsFromArrayArgument()?.Select<ExpressionSyntax, SyntaxNode>(syntax => syntax);
        }

        protected override int AwaitExpressionRawKind { get; } = (int)SyntaxKind.AwaitExpression;

        protected override IEnumerable<ArgumentSyntax> GetArguments(ArgumentListSyntax argumentSyntax)
        {
            return argumentSyntax.Arguments;
        }

        private static ArrayCreationExpressionSyntax CreateArrayCreationExpression(
            SeparatedSyntaxList<ExpressionSyntax> initializers, TypeSyntax typeSyntax)
        {
            var syntaxes = CreateSingleLineLambdaExpressions(initializers);

            var initializer = CollectionInitializer(SeparatedList<ExpressionSyntax>(syntaxes));
            var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier());

            return ArrayCreationExpression(
                Token(SyntaxKind.NewKeyword),
                List<AttributeListSyntax>(),
                typeSyntax,
                null,
                arrayRankSpecifierSyntaxes,
                initializer);
        }

        private static IEnumerable<SingleLineLambdaExpressionSyntax> CreateSingleLineLambdaExpressions(SeparatedSyntaxList<ExpressionSyntax> expressions)
        {
            var singleLineLambdaExpressionSyntaxes = expressions.Select(CreateSingleLineLambdaExpression);

            return singleLineLambdaExpressionSyntaxes;
        }

        private static SingleLineLambdaExpressionSyntax CreateSingleLineLambdaExpression(ExpressionSyntax expressionSyntax)
        {
            var separatedSyntaxList = SeparatedList(SingletonList(Parameter(ModifiedIdentifier(Identifier("x")))));
            var functionLambdaHeader = FunctionLambdaHeader(
                List<AttributeListSyntax>(),
                TokenList(),
                ParameterList(separatedSyntaxList),
                null);

            var lambdaExpression = SingleLineLambdaExpression(
                SyntaxKind.SingleLineFunctionLambdaExpression,
                functionLambdaHeader,
                expressionSyntax.WithLeadingTrivia());
            return lambdaExpression;
        }
    }
}