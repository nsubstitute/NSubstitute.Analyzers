using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class AsyncReceivedInOrderCallbackAnalyzer : AbstractAsyncReceivedInOrderCallbackAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public AsyncReceivedInOrderCallbackAnalyzer()
            : base(CSharp.DiagnosticDescriptorsProvider.Instance)
        {
        }

        protected override int AsyncExpressionRawKind { get; } = (int)SyntaxKind.AsyncKeyword;

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<SyntaxNode> GetArgumentExpressions(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select<ArgumentSyntax, SyntaxNode>(arg => arg.Expression);
        }

        protected override IEnumerable<SyntaxToken?> GetCallbackArgumentSyntaxTokens(SyntaxNode node)
        {
            return node.ChildTokens().Select<SyntaxToken, SyntaxToken?>(token => token);
        }
    }
}