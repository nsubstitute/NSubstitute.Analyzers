using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal sealed class ReEntrantSetupCodeFixProvider : AbstractReEntrantSetupCodeFixProvider<ArgumentSyntax>
{
    protected override string LambdaParameterName => "_";

    protected override ArgumentSyntax UpdateArgumentExpression(ArgumentSyntax argument, SyntaxNode expression) => argument.WithExpression((ExpressionSyntax)expression);

    protected override SyntaxNode GetArgumentExpression(ArgumentSyntax argument) => argument.Expression;

    protected override SyntaxNode CreateArrayCreationExpression(SyntaxNode typeSyntax, IEnumerable<SyntaxNode> elements)
    {
        var initializer = InitializerExpression(SyntaxKind.ArrayInitializerExpression, SeparatedList(elements));

        var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression())));

        var arrayType = ArrayType((TypeSyntax)typeSyntax, arrayRankSpecifierSyntaxes)
            .WithAdditionalAnnotations(Simplifier.Annotation);

        return ArrayCreationExpression(arrayType, initializer);
    }

    protected override IReadOnlyList<ArgumentSyntax> GetArguments(IInvocationOperation invocationOperation) =>
        ((InvocationExpressionSyntax)invocationOperation.Syntax).ArgumentList.Arguments;
}