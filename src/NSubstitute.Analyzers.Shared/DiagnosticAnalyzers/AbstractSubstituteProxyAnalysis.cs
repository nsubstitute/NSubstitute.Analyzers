using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal abstract class AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax> :
    ISubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax>
    where TInvocationExpressionSyntax : SyntaxNode where TExpressionSyntax : SyntaxNode
{
    public ITypeSymbol GetActualProxyTypeSymbol(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
    {
        return GetActualProxyTypeSymbol(substituteContext.SyntaxNodeAnalysisContext.SemanticModel, substituteContext.InvocationExpression, substituteContext.MethodSymbol);
    }

    public ImmutableArray<ITypeSymbol> GetProxySymbols(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
    {
        return GetProxySymbols(substituteContext.SyntaxNodeAnalysisContext.SemanticModel, substituteContext.InvocationExpression, substituteContext.MethodSymbol);
    }

    public ITypeSymbol GetActualProxyTypeSymbol(SemanticModel semanticModel, TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol)
    {
        var proxies = GetProxySymbols(semanticModel, invocationExpressionSyntax, methodSymbol).ToList();

        var classSymbol = proxies.FirstOrDefault(symbol => symbol.TypeKind == TypeKind.Class);

        return classSymbol ?? proxies.FirstOrDefault();
    }

    public ImmutableArray<ITypeSymbol> GetProxySymbols(SemanticModel semanticModel, TInvocationExpressionSyntax invocationExpressionSyntax, IMethodSymbol methodSymbol)
    {
        if (methodSymbol.IsGenericMethod)
        {
            return methodSymbol.TypeArguments;
        }

        var arrayParameters = GetArrayInitializerArguments(invocationExpressionSyntax)?.ToList();

        if (arrayParameters == null)
        {
            return ImmutableArray<ITypeSymbol>.Empty;
        }

        var proxyTypes = GetTypeOfLikeExpressions(arrayParameters)
            .Select(exp =>
                semanticModel
                    .GetTypeInfo(exp.DescendantNodes().First()))
            .Where(model => model.Type != null)
            .Select(model => model.Type)
            .ToImmutableArray();

        return arrayParameters.Count == proxyTypes.Length ? proxyTypes : ImmutableArray<ITypeSymbol>.Empty;
    }

    protected abstract IEnumerable<TExpressionSyntax> GetTypeOfLikeExpressions(IList<TExpressionSyntax> arrayParameters);

    protected abstract IEnumerable<TExpressionSyntax> GetArrayInitializerArguments(TInvocationExpressionSyntax invocationExpressionSyntax);
}