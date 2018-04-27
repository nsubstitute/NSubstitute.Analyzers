using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Extensions
{
    public static class SymbolExtensions
    {
        public static bool IsInterfaceImplementation(this ISymbol method)
        {
            return method.ContainingType.AllInterfaces
                .SelectMany(@interface => @interface.GetMembers()).Any(interfaceMethod =>
                {
                    var findImplementationForInterfaceMember = method.ContainingType.FindImplementationForInterfaceMember(interfaceMethod);
                    return findImplementationForInterfaceMember?.Equals(method) == true;
                });
        }
    }
}