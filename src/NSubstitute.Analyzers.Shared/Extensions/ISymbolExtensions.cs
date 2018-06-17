using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class ISymbolExtensions
    {
        public static bool InternalsVisibleToProxyGenerator(this ISymbol typeSymbol)
        {
            var internalsVisibleToAttribute = typeSymbol.ContainingAssembly.GetAttributes()
                .FirstOrDefault(att =>
                    att.AttributeClass.ToString() == MetadataNames.InternalsVisibleToAttributeFullTypeName);

            return internalsVisibleToAttribute != null &&
                   internalsVisibleToAttribute.ConstructorArguments.Any(arg =>
                       arg.Value.ToString() == MetadataNames.CastleDynamicProxyGenAssembly2Name);
        }

        public static string ToMinimalMethodString(this ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol == null)
            {
                return string.Empty;
            }

            var minimumDisplayString = symbol.ToMinimalDisplayString(semanticModel, 0, SymbolDisplayFormat.FullyQualifiedFormat);

            return $"{symbol.ContainingType}.{minimumDisplayString}";
        }
    }
}