using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class AsyncReceivedInOrderCallbackAnalyzer : AbstractAsyncReceivedInOrderCallbackAnalyzer<SyntaxKind, InvocationExpressionSyntax>
{
    public AsyncReceivedInOrderCallbackAnalyzer()
        : base(VisualBasic.DiagnosticDescriptorsProvider.Instance)
    {
    }

    protected override int AsyncExpressionRawKind { get; } = (int)SyntaxKind.AsyncKeyword;

    protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

    protected override IEnumerable<SyntaxNode> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
    {
        return invocationExpressionSyntax.ArgumentList.Arguments.Select<ArgumentSyntax, SyntaxNode>(arg => arg.GetExpression());
    }

    protected override IEnumerable<SyntaxToken?> GetCallbackArgumentSyntaxTokens(SyntaxNode node)
    {
        switch (node)
        {
            case LambdaExpressionSyntax lambdaExpressionSyntax:
                return lambdaExpressionSyntax.SubOrFunctionHeader.ChildTokens().Select<SyntaxToken, SyntaxToken?>(token => token);
        }

        return Enumerable.Empty<SyntaxToken?>();
    }
}