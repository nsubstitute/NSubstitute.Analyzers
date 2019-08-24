using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal sealed class ConstructorArgumentsForInterfaceCodeFixProvider : AbstractConstructorArgumentsForInterfaceCodeFixProvider<InvocationExpressionSyntax>
    {
        protected override InvocationExpressionSyntax GetInvocationExpressionSyntaxWithEmptyArgumentList(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.WithArgumentList(ArgumentList());
        }

        protected override InvocationExpressionSyntax GetInvocationExpressionSyntaxWithNullConstructorArgument(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var nullSyntax = SimpleArgument(LiteralExpression(SyntaxKind.NothingLiteralExpression, Token(SyntaxKind.NothingKeyword)));
            var seconArgument = invocationExpressionSyntax.ArgumentList.Arguments.Skip(1).First();
            var argumentListSyntax = invocationExpressionSyntax.ArgumentList.ReplaceNode(seconArgument, nullSyntax);
            return invocationExpressionSyntax.WithArgumentList(argumentListSyntax);
        }
    }
}