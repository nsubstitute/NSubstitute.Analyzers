using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class ReEntrantSetupCodeFixProvider : AbstractReEntrantSetupCodeFixProvider<ArgumentSyntax>
{
    protected override string LambdaParameterName => "x";

    protected override IReadOnlyList<ArgumentSyntax> GetArguments(IInvocationOperation invocationOperation) =>
        ((InvocationExpressionSyntax)invocationOperation.Syntax).ArgumentList.Arguments;

    protected override ArgumentSyntax UpdateArgumentExpression(ArgumentSyntax argument, SyntaxNode expression)
    {
        return argument switch
        {
            SimpleArgumentSyntax simpleArgumentSyntax => simpleArgumentSyntax.WithExpression((ExpressionSyntax)expression),
            _ => argument
        };
    }

    protected override SyntaxNode GetArgumentExpression(ArgumentSyntax argument) => argument.GetExpression();

    protected override SyntaxNode CreateArrayCreationExpression(SyntaxNode typeSyntax, IEnumerable<SyntaxNode> elements)
    {
        var initializer = CollectionInitializer(SeparatedList(elements));
        var arrayRankSpecifierSyntaxes = SingletonList(ArrayRankSpecifier());

        return ArrayCreationExpression(
            Token(SyntaxKind.NewKeyword),
            new SyntaxList<AttributeListSyntax>(),
            (TypeSyntax)typeSyntax,
            null,
            arrayRankSpecifierSyntaxes,
            initializer);
    }
}