using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal sealed class ConstructorArgumentsForInterfaceCodeFixProvider : AbstractConstructorArgumentsForInterfaceCodeFixProvider
{
    protected override SyntaxNode GetInvocationExpressionSyntaxWithEmptyArgumentList(IInvocationOperation invocationOperation)
    {
        var invocationExpression = (InvocationExpressionSyntax)invocationOperation.Syntax;

        return invocationExpression.WithArgumentList(ArgumentList());
    }

    protected override SyntaxNode GetInvocationExpressionSyntaxWithNullConstructorArgument(IInvocationOperation invocationOperation)
    {
        var invocationExpressionSyntax = (InvocationExpressionSyntax)invocationOperation.Syntax;
        var arguments = invocationOperation.Arguments.Select(argumentOperation =>
        {
            var argumentSyntax = (ArgumentSyntax)argumentOperation.Syntax;
            if (argumentOperation.Parameter.Ordinal > 0)
            {
                argumentSyntax = argumentSyntax.WithExpression(LiteralExpression(SyntaxKind.NullLiteralExpression));
            }

            return argumentSyntax;
        });

        return invocationExpressionSyntax.WithArgumentList(ArgumentList(SeparatedList(arguments)));
    }
}