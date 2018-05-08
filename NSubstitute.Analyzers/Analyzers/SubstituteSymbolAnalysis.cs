using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Analyzers
{
    public class SubstituteSymbolAnalysis
    {
        public static bool IsPartOfInterfaceImplementation(ISymbol symbol)
        {
            return symbol.ContainingType.AllInterfaces
                .SelectMany(@interface => @interface.GetMembers()).Any(interfaceMethod =>
                {
                    var interfaceSymbol = symbol.ContainingType.FindImplementationForInterfaceMember(interfaceMethod);
                    return IsSame(interfaceSymbol, symbol);
                });
        }

        private static bool IsSame(ISymbol interfaceSymbol, ISymbol symbol)
        {
            if (interfaceSymbol == null)
            {
                return false;
            }

            if (interfaceSymbol.Equals(symbol))
            {
                return true;
            }

            if (interfaceSymbol.Kind == SymbolKind.Method && symbol.Kind == SymbolKind.Method)
            {
                var left = (IMethodSymbol) interfaceSymbol;
                var right = (IMethodSymbol) symbol;

                return left.ConstructedFrom?.Equals(right.ConstructedFrom) ?? false;
            }

            return false;
        }
    }
}