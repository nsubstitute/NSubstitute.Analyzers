using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal abstract class AbstractSubstituteProxyAnalysis<TInvocationExpressionSyntax> : ISubstituteProxyAnalysis<TInvocationExpressionSyntax>
        where TInvocationExpressionSyntax : SyntaxNode
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

            var operation = (IInvocationOperation)semanticModel.GetOperation(invocationExpressionSyntax);

            var argument = operation.Arguments.First();
            var typeSymbols = ArgType(argument);

            return typeSymbols;
        }

        // TODO remove copy pasting
        private ImmutableArray<ITypeSymbol> TypeSymbols(IArrayCreationOperation arrayInitializerOperation)
        {
            return arrayInitializerOperation.Initializer.ElementValues.OfType<ITypeOfOperation>()
                .Select(op => op.TypeOperand)
                .ToImmutableArray();
        }

        private ImmutableArray<ITypeSymbol> ArgType(IArgumentOperation x)
        {
            if (x.Value is IArrayCreationOperation arrayInitializerOperation)
            {
                var typeSymbols = TypeSymbols(arrayInitializerOperation);
                return arrayInitializerOperation.Initializer.ElementValues.Length == typeSymbols.Length
                    ? typeSymbols
                    : ImmutableArray<ITypeSymbol>.Empty;
            }

            return ImmutableArray<ITypeSymbol>.Empty;
        }
    }
}