using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class ReEntrantSetupAnalyzer : AbstractReEntrantSetupAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public ReEntrantSetupAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override AbstractReEntrantCallFinder GetReEntrantCallFinder()
        {
            return new ReEntrantCallFinder();
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        protected override IEnumerable<SyntaxNode> ExtractArguments(InvocationExpressionSyntax invocationExpressionSyntax)
        {
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.Expression);
        }
    }
}