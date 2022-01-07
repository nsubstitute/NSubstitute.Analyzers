using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.Extensions;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class ReEntrantSetupCodeFixProvider : AbstractReEntrantSetupCodeFixProvider<ArgumentListSyntax, ArgumentSyntax>
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

    protected override ArgumentSyntax CreateUpdatedParamsArgumentSyntaxNode(SyntaxGenerator syntaxGenerator, ITypeSymbol typeSymbol, ArgumentSyntax argumentSyntaxNode)
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
                    syntaxGenerator,
                    typeSymbol,
                    arrayCreationExpressionSyntax.Initializer.Initializers);
                break;
            case CollectionInitializerSyntax implicitArrayCreationExpressionSyntax:
                resultArrayCreationExpressionSyntax = CreateArrayCreationExpression(
                    syntaxGenerator,
                    typeSymbol,
                    implicitArrayCreationExpressionSyntax.Initializers);
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
        SyntaxGenerator syntaxGenerator,
        ITypeSymbol typeSymbol,
        SeparatedSyntaxList<ExpressionSyntax> initializers)
    {
        var typeNode = CreateTypeNode(syntaxGenerator, typeSymbol);
        var syntaxes = CreateSingleLineLambdaExpressions(initializers);

        var initializer = CollectionInitializer(syntaxes);
        var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier());

        return ArrayCreationExpression(Token(SyntaxKind.NewKeyword), new SyntaxList<AttributeListSyntax>(), typeNode, null, arrayRankSpecifierSyntaxes, initializer);
    }

    private static SeparatedSyntaxList<ExpressionSyntax> CreateSingleLineLambdaExpressions(SeparatedSyntaxList<ExpressionSyntax> expressions)
    {
        var singleLineLambdaExpressionSyntaxes = expressions.Select<ExpressionSyntax, ExpressionSyntax>(CreateSingleLineLambdaExpression);

        return SeparatedList(singleLineLambdaExpressionSyntaxes);
    }

    private static SingleLineLambdaExpressionSyntax CreateSingleLineLambdaExpression(ExpressionSyntax expressionSyntax)
    {
        var separatedSyntaxList = SeparatedList(SingletonList(Parameter(ModifiedIdentifier(Identifier("x")))));
        var functionLambdaHeader = FunctionLambdaHeader(
            new SyntaxList<AttributeListSyntax>(),
            TokenList(),
            ParameterList(separatedSyntaxList),
            null);

        var lambdaExpression = SingleLineLambdaExpression(
            SyntaxKind.SingleLineFunctionLambdaExpression,
            functionLambdaHeader,
            expressionSyntax.WithLeadingTrivia());
        return lambdaExpression;
    }

    private static TypeSyntax CreateTypeNode(SyntaxGenerator syntaxGenerator, ITypeSymbol type)
    {
        var typeSyntax = (TypeSyntax)syntaxGenerator.TypeExpression(type);
        return typeSyntax.WithAdditionalAnnotations(Simplifier.Annotation);
    }
}