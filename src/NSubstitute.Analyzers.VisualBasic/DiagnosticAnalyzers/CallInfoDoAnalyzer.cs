using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.VisualBasic.Extensions;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers
{
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

            if (syntaxNodeContext.SemanticModel.GetSymbolInfo(parentInvocationExpression).Symbol is IMethodSymbol parentInvocationSymbol)
            {
                var argumentExpression = parentInvocationSymbol.MethodKind == MethodKind.ReducedExtension
                       ? parentInvocationExpression.ArgumentList.Arguments.First().GetExpression()
                       : parentInvocationExpression.ArgumentList.Arguments.Skip(1).First().GetExpression();

                return WhenSubstituteCallFinder.Find(syntaxNodeContext, argumentExpression).FirstOrDefault();
            }

            return null;
        }
    }
}