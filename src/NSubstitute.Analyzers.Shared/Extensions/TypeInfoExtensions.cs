using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class TypeInfoExtensions
{
    public static bool IsCallInfoDelegate(this TypeInfo typeInfo, SemanticModel semanticModel)
    {
        var typeSymbol = typeInfo.Type ?? typeInfo.ConvertedType;
        return typeSymbol.IsCallInfoDelegate(semanticModel);
    }

    public static bool IsCallInfoDelegate(this ITypeSymbol typeSymbol, SemanticModel semanticModel)
    {
        var isCalledViaDelegate = typeSymbol != null &&
                                  typeSymbol.TypeKind == TypeKind.Delegate &&
                                  typeSymbol is INamedTypeSymbol namedTypeSymbol &&
                                  (namedTypeSymbol.ConstructedFrom.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Func`2")) ||
                                   namedTypeSymbol.ConstructedFrom.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Action`1"))) &&
                                  IsCallInfoSymbol(namedTypeSymbol.TypeArguments.First());

        return isCalledViaDelegate;
    }

    public static bool IsCallInfoSymbol(this ITypeSymbol symbol)
    {
        return IsCallInfoSymbolInternal(symbol) || IsCallInfoSymbolInternal(symbol.BaseType);
    }

    private static bool IsCallInfoSymbolInternal(ISymbol symbol)
    {
        return symbol != null &&
               symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
               symbol.ToString().Equals(MetadataNames.NSubstituteCallInfoFullTypeName, StringComparison.OrdinalIgnoreCase);
    }
}