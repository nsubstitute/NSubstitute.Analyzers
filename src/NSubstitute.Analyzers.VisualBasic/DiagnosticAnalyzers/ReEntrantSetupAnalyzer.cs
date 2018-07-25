using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
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
            return invocationExpressionSyntax.ArgumentList.Arguments.Select(arg => arg.GetExpression());
        }
    }
}