using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class ISymbolExtensions
    {
        public static bool CanBeSetuped(this ISymbol symbol)
        {
            return IsInterfaceMember(symbol) || IsVirtual(symbol);
        }

        public static bool MemberVisibleToProxyGenerator(this ISymbol symbol)
        {
            return symbol.DeclaredAccessibility != Accessibility.Internal || symbol.InternalsVisibleToProxyGenerator();
        }

        public static bool InternalsVisibleToProxyGenerator(this ISymbol typeSymbol)
        {
            return typeSymbol.ContainingAssembly != null &&
                   typeSymbol.ContainingAssembly.GetAttributes()
                       .Any(att => att.AttributeClass.ToString() == MetadataNames.InternalsVisibleToAttributeFullTypeName &&
                                   att.ConstructorArguments.Any(arg => arg.Value != null && AssemblyIdentity.TryParseDisplayName(arg.Value.ToString(), out var identity) &&
                                                                       identity.Name == MetadataNames.CastleDynamicProxyGenAssembly2Name));
        }

        public static string ToMinimalMethodString(this ISymbol symbol, SemanticModel semanticModel)
        {
            if (symbol == null)
            {
                return string.Empty;
            }

            var minimumDisplayString =
                symbol.ToMinimalDisplayString(semanticModel, 0, SymbolDisplayFormat.FullyQualifiedFormat);

            return $"{symbol.ContainingType}.{minimumDisplayString}";
        }

        public static bool IsLocal(this ISymbol symbol)
        {
            return symbol != null && symbol.Kind == SymbolKind.Local;
        }

        private static bool IsInterfaceMember(ISymbol symbol)
        {
            return symbol?.ContainingType?.TypeKind == TypeKind.Interface;
        }

        private static bool IsVirtual(ISymbol symbol)
        {
            var isVirtual = symbol.IsVirtual
                            || (symbol.IsOverride && !symbol.IsSealed)
                            || symbol.IsAbstract;

            return isVirtual;
        }
    }
}