using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class SubstituteProxyAnalysis : ISubstituteProxyAnalysis
{
    public static SubstituteProxyAnalysis Instance { get; } = new ();

    public ITypeSymbol GetActualProxyTypeSymbol(IInvocationOperation invocationOperation)
    {
        var proxies = GetProxySymbols(invocationOperation).ToList();

        var classSymbol = proxies.FirstOrDefault(symbol => symbol.TypeKind == TypeKind.Class);

        return classSymbol ?? proxies.FirstOrDefault();
    }

    public ImmutableArray<ITypeSymbol> GetProxySymbols(IInvocationOperation invocationOperation)
    {
        if (invocationOperation.TargetMethod.IsGenericMethod)
        {
            return invocationOperation.TargetMethod.TypeArguments;
        }

        var arrayParameters = GetArrayInitializerArguments(invocationOperation);

        if (arrayParameters == null)
        {
            return ImmutableArray<ITypeSymbol>.Empty;
        }

        var proxyTypes = arrayParameters.ElementValues.OfType<ITypeOfOperation>()
            .Select(typeOfOperation => typeOfOperation.TypeOperand)
            .ToImmutableArray();

        // get typeof like expressions
        return arrayParameters.ElementValues.Length == proxyTypes.Length
            ? proxyTypes
            : ImmutableArray<ITypeSymbol>.Empty;
    }

    private IArrayInitializerOperation GetArrayInitializerArguments(IInvocationOperation invocationOperation)
    {
        return invocationOperation.Arguments.FirstOrDefault()?.Value switch
        {
            IArrayCreationOperation operation => operation.Initializer,
            _ => null
        };
    }
}