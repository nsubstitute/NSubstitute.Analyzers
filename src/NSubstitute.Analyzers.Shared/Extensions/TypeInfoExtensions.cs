using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    public static class TypeInfoExtensions
    {
        public static bool IsCallInfoDelegate(this TypeInfo typeInfo, SemanticModel semanticModel)
        {
            var typeSymbol = typeInfo.Type ?? typeInfo.ConvertedType;
            var isCalledViaDelegate = typeSymbol != null &&
                                      typeSymbol.TypeKind == TypeKind.Delegate &&
                                      typeSymbol is INamedTypeSymbol namedTypeSymbol &&
                                      namedTypeSymbol.ConstructedFrom.Equals(semanticModel.Compilation.GetTypeByMetadataName("System.Func`2")) &&
                                      IsCallInfoParameter(namedTypeSymbol.TypeArguments.First());

            return isCalledViaDelegate;
        }

        private static bool IsCallInfoParameter(ITypeSymbol symbol)
        {
            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ToString().Equals(MetadataNames.NSubstituteCoreFullTypeName, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}