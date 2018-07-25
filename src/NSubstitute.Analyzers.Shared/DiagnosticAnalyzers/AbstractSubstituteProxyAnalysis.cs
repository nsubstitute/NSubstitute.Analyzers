using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax, TExpressionSyntax>
        where TInvocationExpressionSyntax : SyntaxNode
        where TExpressionSyntax : SyntaxNode
    {
        public ITypeSymbol GetActualProxyTypeSymbol(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            var proxies = GetProxySymbols(substituteContext).ToList();

            var classSymbol = proxies.FirstOrDefault(symbol => symbol.TypeKind == TypeKind.Class);

            return classSymbol ?? proxies.FirstOrDefault();
        }

        public ImmutableArray<ITypeSymbol> GetProxySymbols(SubstituteContext<TInvocationExpressionSyntax> substituteContext)
        {
            if (substituteContext.MethodSymbol.IsGenericMethod)
            {
                return substituteContext.MethodSymbol.TypeArguments;
            }

            var arrayParameters = GetArrayInitializerArguments(substituteContext.InvocationExpression)?.ToList();

            if (arrayParameters == null)
            {
                return ImmutableArray<ITypeSymbol>.Empty;
            }

            var proxyTypes = GetTypeOfLikeExpressions(arrayParameters)
                .Select(exp =>
                    substituteContext.SyntaxNodeAnalysisContext.SemanticModel
                        .GetTypeInfo(exp.DescendantNodes().First()))
                .Where(model => model.Type != null)
                .Select(model => model.Type)
                .ToImmutableArray();

            return arrayParameters.Count == proxyTypes.Length ? proxyTypes : ImmutableArray<ITypeSymbol>.Empty;
        }

        protected abstract IEnumerable<TExpressionSyntax> GetTypeOfLikeExpressions(IList<TExpressionSyntax> arrayParameters);

        protected abstract IEnumerable<TExpressionSyntax> GetArrayInitializerArguments(TInvocationExpressionSyntax invocationExpressionSyntax);
    }
}