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

        public static string ToSimplifiedMethodString(this ISymbol symbol)
        {
            if (symbol == null)
            {
                return string.Empty;
            }

            // consider using span
            var defaultString = symbol.ToString();
            var bracketIndex = defaultString.IndexOf('(');
            return bracketIndex > -1 ? defaultString.Substring(0, bracketIndex) : defaultString;
        }
    }
}