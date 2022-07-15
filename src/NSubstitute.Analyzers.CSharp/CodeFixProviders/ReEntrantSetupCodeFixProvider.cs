using System;
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

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

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
        ArgumentSyntax argumentSyntaxNode)
    {
        var expression = argumentSyntaxNode.Expression;

        var resultArrayCreationExpressionSyntax = expression switch
        {
            ArrayCreationExpressionSyntax arrayCreationExpressionSyntax => CreateArrayCreationExpression(
                syntaxGenerator, typeSymbol, arrayCreationExpressionSyntax.Initializer),
            ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax =>
                CreateArrayCreationExpression(syntaxGenerator, typeSymbol, implicitArrayCreationExpressionSyntax.Initializer),
            _ => throw new ArgumentException($"{argumentSyntaxNode.Kind()} is not recognized as array initialization", nameof(argumentSyntaxNode))
        };

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
        SyntaxGenerator syntaxGenerator,
        ITypeSymbol typeSymbol,
        InitializerExpressionSyntax initializerExpressionSyntax)
    {
        var arrayType = CreateArrayTypeNode(syntaxGenerator, typeSymbol);
        var syntaxes = CreateSimpleLambdaExpressions(initializerExpressionSyntax);

        var initializer = InitializerExpression(SyntaxKind.ArrayInitializerExpression, syntaxes);

        return ArrayCreationExpression(arrayType, initializer);
    }

    private static SeparatedSyntaxList<ExpressionSyntax> CreateSimpleLambdaExpressions(InitializerExpressionSyntax initializerExpressionSyntax)
    {
        var expressions = initializerExpressionSyntax.Expressions.Select<ExpressionSyntax, ExpressionSyntax>(CreateSimpleLambdaExpressionNode);
        return SeparatedList(expressions);
    }

    private static ArrayTypeSyntax CreateArrayTypeNode(SyntaxGenerator syntaxGenerator, ITypeSymbol type)
    {
        var typeSyntax = (TypeSyntax)syntaxGenerator.TypeExpression(type);

        var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())));

        return ArrayType(typeSyntax, arrayRankSpecifierSyntaxes).WithAdditionalAnnotations(Simplifier.Annotation);
    }
}