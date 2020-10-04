using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstitutionNodeFinder<TInvocationExpressionSyntax> : ISubstitutionNodeFinder<TInvocationExpressionSyntax>
        where TInvocationExpressionSyntax : SyntaxNode
    {
        public IEnumerable<SyntaxNode> Find(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol invocationExpressionSymbol = null)
        {
            if (invocationOperation == null)
            {
                return Enumerable.Empty<SyntaxNode>();
            }

            var invocationExpression = invocationOperation.Syntax;
            invocationExpressionSymbol ??= syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            if (invocationExpressionSymbol == null || invocationExpressionSymbol.ContainingAssembly.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) == false)
            {
                return Enumerable.Empty<SyntaxNode>();
            }

            if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteDoMethod, StringComparison.Ordinal))
            {
                var operation = invocationOperation.GetSubstituteOperation();
                return FindForWhenExpression(syntaxNodeContext, operation as IInvocationOperation);
            }

            if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenMethod, StringComparison.Ordinal) ||
                invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteWhenForAnyArgsMethod, StringComparison.Ordinal))
            {
                return FindForWhenExpression(syntaxNodeContext, invocationOperation, invocationExpressionSymbol);
            }

            if (invocationExpressionSymbol.Name.Equals(MetadataNames.NSubstituteAndDoesMethod, StringComparison.Ordinal))
            {
                var substitution = FindForAndDoesExpression(syntaxNodeContext, invocationOperation, invocationExpressionSymbol);
                return substitution != null ? new[] { substitution } : Enumerable.Empty<SyntaxNode>();
            }

            var standardSubstitution = FindForStandardExpression(invocationOperation);

            return standardSubstitution != null ? new[] { standardSubstitution } : Enumerable.Empty<SyntaxNode>();
        }

        public IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol whenInvocationSymbol = null)
        {
            if (invocationOperation == null)
            {
                yield break;
            }

            var invocationExpression = invocationOperation.Syntax;
            whenInvocationSymbol ??= syntaxNodeContext.SemanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;

            if (whenInvocationSymbol == null)
            {
                yield break;
            }

            var typeSymbol = whenInvocationSymbol.TypeArguments.FirstOrDefault() ?? whenInvocationSymbol.ReceiverType;
            foreach (var syntaxNode in FindForWhenExpressionInternal(syntaxNodeContext, (TInvocationExpressionSyntax)invocationExpression, whenInvocationSymbol))
            {
                var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
                if (symbol != null && typeSymbol != null && ContainsSymbol(typeSymbol, symbol))
                {
                    yield return syntaxNode;
                }
            }
        }

        public SyntaxNode FindForAndDoesExpression(SyntaxNodeAnalysisContext syntaxNodeContext, IInvocationOperation invocationOperation, IMethodSymbol invocationExpressionSymbol)
        {
            if (!(invocationOperation.GetSubstituteOperation() is IInvocationOperation parentInvocationExpression))
            {
                return null;
            }

            return FindForStandardExpression(parentInvocationExpression);
        }

        public SyntaxNode FindForStandardExpression(IInvocationOperation invocationOperation)
        {
            return invocationOperation.GetSubstituteOperation().Syntax;
        }

        public abstract IEnumerable<SyntaxNode> FindForReceivedInOrderExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax receivedInOrderExpression, IMethodSymbol receivedInOrderInvocationSymbol = null);

        protected abstract IEnumerable<SyntaxNode> FindForWhenExpressionInternal(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax whenInvocationExpression, IMethodSymbol whenInvocationSymbol);

        private bool ContainsSymbol(ITypeSymbol containerSymbol, ISymbol symbol)
        {
            return GetBaseTypesAndThis(containerSymbol).Any(typeSymbol => typeSymbol == symbol.ContainingType);
        }

        private static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }
    }
}