using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Analyzers
{
    public class SubstituteSymbolAnalysis
    {
        public static bool IsInterfaceImplementation(ISymbol method)
        {
            return method.ContainingType.AllInterfaces
                .SelectMany(@interface => @interface.GetMembers()).Any(interfaceMethod =>
                {
                    var findImplementationForInterfaceMember =
                        method.ContainingType.FindImplementationForInterfaceMember(interfaceMethod);
                    return IsSame(findImplementationForInterfaceMember, method);
                });
        }

        private static bool IsSame(ISymbol findImplementationForInterfaceMember, ISymbol method)
        {
            if (findImplementationForInterfaceMember == null)
            {
                return false;
            }

            if (findImplementationForInterfaceMember.Equals(method))
            {
                return true;
            }

            if (findImplementationForInterfaceMember.Kind == SymbolKind.Method && method.Kind == SymbolKind.Method)
            {
                var left = (IMethodSymbol) findImplementationForInterfaceMember;
                var right = (IMethodSymbol) method;

                return left.ConstructedFrom.Equals(right.ConstructedFrom);
            }

            return false;
        }
    }
}