using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstitutionNodeFinder<TInvocationExpressionSyntax> : ISubstitutionNodeFinder<TInvocationExpressionSyntax>
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

        var standardSubstitution = FindForStandardExpression(invocationExpression, invocationExpressionSymbol);

        return standardSubstitution != null ? new[] { standardSubstitution } : Enumerable.Empty<SyntaxNode>();
    }

    public IEnumerable<SyntaxNode> FindForWhenExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax whenInvocationExpression, IMethodSymbol whenInvocationSymbol = null)
    {
        if (whenInvocationExpression == null)
        {
            yield break;
        }

        whenInvocationSymbol = whenInvocationSymbol ?? syntaxNodeContext.SemanticModel.GetSymbolInfo(whenInvocationExpression).Symbol as IMethodSymbol;

        if (whenInvocationSymbol == null)
        {
            yield break;
        }

        var typeSymbol = whenInvocationSymbol.TypeArguments.FirstOrDefault() ?? whenInvocationSymbol.ReceiverType;
        foreach (var syntaxNode in FindForWhenExpressionInternal(syntaxNodeContext, whenInvocationExpression, whenInvocationSymbol))
        {
            var symbol = syntaxNodeContext.SemanticModel.GetSymbolInfo(syntaxNode).Symbol;
            if (symbol != null && typeSymbol != null && ContainsSymbol(typeSymbol, symbol))
            {
                yield return syntaxNode;
            }
        }
    }

    public abstract SyntaxNode FindForAndDoesExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax invocationExpression, IMethodSymbol invocationExpressionSymbol = null);

    public abstract SyntaxNode FindForStandardExpression(TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol invocationExpressionSymbol = null);

    public SyntaxNode FindForStandardExpression(IInvocationOperation invocationOperation)
        {
            if (invocationOperation.Instance != null)
            {
                return invocationOperation.Instance.Syntax;
            }

            return invocationOperation.Arguments.First(arg => arg.Parameter.Ordinal == 0).Value.Syntax;
        }

    public abstract IEnumerable<SyntaxNode> FindForReceivedInOrderExpression(SyntaxNodeAnalysisContext syntaxNodeContext, TInvocationExpressionSyntax receivedInOrderExpression, IMethodSymbol receivedInOrderInvocationSymbol = null);

    protected abstract TInvocationExpressionSyntax GetParentInvocationExpression(TInvocationExpressionSyntax invocationExpressionSyntax);

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