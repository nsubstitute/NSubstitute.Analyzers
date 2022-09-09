using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class ISymbolExtensions
{
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

    public static string ToMinimalSymbolString(this ISymbol symbol, SemanticModel semanticModel)
    {
        return symbol.ToMinimalDisplayString(semanticModel, 0, SymbolDisplayFormat.CSharpErrorMessageFormat);
    }
}