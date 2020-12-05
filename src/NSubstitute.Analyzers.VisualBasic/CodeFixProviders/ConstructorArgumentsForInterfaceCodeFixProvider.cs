using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal sealed class ConstructorArgumentsForInterfaceCodeFixProvider : AbstractConstructorArgumentsForInterfaceCodeFixProvider<InvocationExpressionSyntax>
    {
        protected override SyntaxNode GetExpression(InvocationExpressionSyntax invocationExpressionSyntax) => invocationExpressionSyntax.Expression;

        protected override bool HasNamedArguments(InvocationExpressionSyntax invocationExpressionSyntax) =>
            invocationExpressionSyntax.ArgumentList.Arguments.Any(arg => arg.IsNamed);
    }
}