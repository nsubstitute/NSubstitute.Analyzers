using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class ReEntrantSetupAnalyzer : AbstractReEntrantSetupAnalyzer<SyntaxKind, InvocationExpressionSyntax, ArgumentSyntax>
    {
        public ReEntrantSetupAnalyzer()
            : base(NSubstitute.Analyzers.CSharp.DiagnosticDescriptorsProvider.Instance, ReEntrantCallFinder.Instance)
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<SyntaxNode> GetExpressionsFromArrayInitializer(ArgumentSyntax syntaxNode)
        {
            return syntaxNode.Expression.GetParameterExpressionsFromArrayArgument();
        }

        protected override IEnumerable<ArgumentSyntax> GetArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments;
        }

        protected override SyntaxNode GetArgumentExpression(ArgumentSyntax argumentSyntax)
        {
            return argumentSyntax.Expression;
        }
    }
}