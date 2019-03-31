using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal class CallInfoDoAnalyzer : CallInfoAnalyzer
    {
        private readonly Lazy<WhenSubstituteCallFinder> _whenSubstituteCallFinderProxy = new Lazy<WhenSubstituteCallFinder>(() => new WhenSubstituteCallFinder());

        private WhenSubstituteCallFinder WhenSubstituteCallFinder => _whenSubstituteCallFinderProxy.Value;

        protected override bool SupportsCallInfo(SyntaxNodeAnalysisContext syntaxNodeContext, InvocationExpressionSyntax syntax, IMethodSymbol methodSymbol)
        {
            if (methodSymbol.Name != "Do")
            {
                return false;
            }

            var allArguments = GetArgumentExpressions(syntax);
            return allArguments.Any(arg => syntaxNodeContext.SemanticModel.GetTypeInfo(arg).IsCallInfoDelegate(syntaxNodeContext.SemanticModel));
        }

        protected override SyntaxNode GetSubstituteCall(SyntaxNodeAnalysisContext syntaxNodeContext, IMethodSymbol methodSymbol, InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var parentInvocationExpression = invocationExpressionSyntax.GetParentInvocationExpression();

            if (parentInvocationExpression == null)
            {
                return null;
            }

            return WhenSubstituteCallFinder.Find(syntaxNodeContext, parentInvocationExpression).FirstOrDefault();
        }
    }
}