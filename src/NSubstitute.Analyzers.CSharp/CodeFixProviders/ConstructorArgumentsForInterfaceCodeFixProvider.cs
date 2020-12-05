using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class ConstructorArgumentsForInterfaceCodeFixProvider : AbstractConstructorArgumentsForInterfaceCodeFixProvider<InvocationExpressionSyntax>
    {
        protected override SyntaxNode GetExpression(InvocationExpressionSyntax invocationExpressionSyntax) =>
            invocationExpressionSyntax.Expression;

        protected override bool HasNamedArguments(InvocationExpressionSyntax invocationExpressionSyntax) =>
            invocationExpressionSyntax.ArgumentList.Arguments.Any(arg => arg.NameColon != null);
    }
}