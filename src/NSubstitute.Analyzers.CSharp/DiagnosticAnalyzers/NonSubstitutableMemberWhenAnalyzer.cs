using System;
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
    internal class NonSubstitutableMemberWhenAnalyzer : AbstractNonSubstitutableMemberWhenAnalyzer<SyntaxKind, InvocationExpressionSyntax>
    {
        public NonSubstitutableMemberWhenAnalyzer()
            : base(new DiagnosticDescriptorsProvider())
        {
        }

        protected override SyntaxKind InvocationExpressionKind { get; } = SyntaxKind.InvocationExpression;

        private readonly Lazy<WhenSubstituteCallFinder> _whenSubstituteCallFinderProxy = new Lazy<WhenSubstituteCallFinder>(() => new WhenSubstituteCallFinder());

        private WhenSubstituteCallFinder WhenSubstituteCallFinder => _whenSubstituteCallFinderProxy.Value;

        protected override IEnumerable<SyntaxNode> GetExpressionsForAnalysys(SyntaxNodeAnalysisContext syntaxNodeAnalysisContext, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var argumentListArguments = invocationExpressionSyntax.ArgumentList.Arguments;
            var argumentSyntax = methodSymbol.MethodKind == MethodKind.ReducedExtension ? argumentListArguments.First() : argumentListArguments.Skip(1).First();
            return WhenSubstituteCallFinder.Find(syntaxNodeAnalysisContext, argumentSyntax.Expression);
        }
    }
}