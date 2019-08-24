using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class ConstructorArgumentsForInterfaceCodeFixProvider : AbstractConstructorArgumentsForInterfaceCodeFixProvider<InvocationExpressionSyntax>
    {
        protected override InvocationExpressionSyntax GetInvocationExpressionSyntaxWithEmptyArgumentList(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.WithArgumentList(ArgumentList());
        }

        protected override InvocationExpressionSyntax GetInvocationExpressionSyntaxWithNullConstructorArgument(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var nullSyntax = Argument(LiteralExpression(SyntaxKind.NullLiteralExpression));
            var seconArgument = invocationExpressionSyntax.ArgumentList.Arguments.Skip(1).First();
            var argumentListSyntax = invocationExpressionSyntax.ArgumentList.ReplaceNode(seconArgument, nullSyntax);
            return invocationExpressionSyntax.WithArgumentList(argumentListSyntax);
        }
    }
}