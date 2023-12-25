using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class TypeInfoExtensions
{
    public static bool IsCallInfoDelegate(this ITypeSymbol? typeSymbol, Compilation compilation)
    {
        var isCalledViaDelegate = typeSymbol != null &&
                                  typeSymbol.TypeKind == TypeKind.Delegate &&
                                  typeSymbol is INamedTypeSymbol namedTypeSymbol &&
                                  (namedTypeSymbol.ConstructedFrom.Equals(compilation.GetTypeByMetadataName("System.Func`2")) ||
                                   namedTypeSymbol.ConstructedFrom.Equals(compilation.GetTypeByMetadataName("System.Action`1"))) &&
                                  IsCallInfoSymbol(namedTypeSymbol.TypeArguments.First());

        return isCalledViaDelegate;
    }

    public static bool IsArgAnyType(this ITypeSymbol? typeSymbol, Compilation compilation)
    {
        return typeSymbol != null
               && typeSymbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true
               && typeSymbol.ToString().Equals(MetadataNames.NSubstituteArgAnyTypeFullTypeName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCallInfoSymbol(this ITypeSymbol symbol)
    {
        return IsCallInfoSymbolInternal(symbol) || IsCallInfoSymbolInternal(symbol.BaseType);
    }

    private static bool IsCallInfoSymbolInternal(ISymbol? symbol)
    {
        return symbol != null &&
               symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
               symbol.ToString().Equals(MetadataNames.NSubstituteCallInfoFullTypeName, StringComparison.OrdinalIgnoreCase);
    }
}