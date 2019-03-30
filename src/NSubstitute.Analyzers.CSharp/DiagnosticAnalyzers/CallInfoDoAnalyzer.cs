using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.Extensions;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers
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
                       ? parentInvocationExpression.ArgumentList.Arguments.First().Expression
                       : parentInvocationExpression.ArgumentList.Arguments.Skip(1).First().Expression;

                return WhenSubstituteCallFinder.Find(syntaxNodeContext, argumentExpression).FirstOrDefault();
            }

            return null;
        }
    }
}