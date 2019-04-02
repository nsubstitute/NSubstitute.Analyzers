using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstitutionNodeFinder<TInvocationExpressionSyntax>
        where TInvocationExpressionSyntax : SyntaxNode
    {
        public IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol = null)
        {
            invocationExpressionSymbol = invocationExpressionSymbol ?? syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            if (invocationExpression == null || invocationExpressionSymbol == null || invocationExpressionSymbol.ContainingAssembly.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == false)
            {
                return Enumerable.Empty<SyntaxNode>();
            }

            if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteDoMethod, StringComparison.Ordinal))
            {
                var parentInvocationExpression = GetParentInvocationExpression(invocationExpression);
                return FindForWhenExpression(syntaxNodeContext, parentInvocationExpression);
            }

            if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenMethod, StringComparison.Ordinal) ||
                invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenForAnyArgsMethod, StringComparison.Ordinal))
            {
                return FindForWhenExpression(syntaxNodeContext, invocationExpression, invocationExpressionSymbol);
            }

            if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteAndDoesMethod, StringComparison.Ordinal))
            {
                var substitution = FindForAndDoesExpression(syntaxNodeContext, invocationExpression, invocationExpressionSymbol);
                return substitution != null ? new[] { substitution } : Enumerable.Empty<SyntaxNode>();
            }

            var standardSubstitution = FindForStandardSubstitution(invocationExpression, invocationExpressionSymbol);

            return standardSubstitution != null ? new[] { standardSubstitution } : Enumerable.Empty<SyntaxNode>();
        }

        public abstract IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax whenInvocationExpression, IMethodSymbol whenInvocationSymbol = null);

        public abstract SyntaxNode FindForAndDoesExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol);

        protected abstract TInvocationExpressionSyntax GetParentInvocationExpression(TInvocationExpressionSyntax invocationExpressionSyntax);

        protected abstract SyntaxNode FindForStandardSubstitution(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol);
    }
}