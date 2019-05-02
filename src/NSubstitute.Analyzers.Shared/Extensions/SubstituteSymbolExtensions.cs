using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SubstituteSymbolExtensions
    {
        public static bool IsReturnLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.ReturnsMethodNames);
        }

        public static bool IsThrowLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.ThrowsMethodNames);
        }

        public static bool IsReceivedLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.ReceivedMethodNames);
        }

        public static bool IsWhenLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.WhenMethodNames);
        }

        public static bool IsArgMatcherLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.ArgMatchersMethodNames) || IsMember(symbol, MetadataNames.ArgMatchersCompatMethodNames);
        }

        public static bool IsArgInvokerLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.ArgInvokersMethodNames) || IsMember(symbol, MetadataNames.ArgInvokersCompatMethodNames);
        }

        public static bool IsReceivedInOrderMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.NSubstituteInOrderMethod, MetadataNames.NSubstituteReceivedFullTypeName);
        }

        private static bool IsMember(this ISymbol symbol, IReadOnlyDictionary<string, string> memberTypeMap)
        {
            if (symbol == null)
            {
                return false;
            }

            if (memberTypeMap.TryGetValue(symbol.Name, out var containingType) == false)
            {
                return false;
            }

            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }

        private static bool IsMember(ISymbol symbol, string name, string containingType)
        {
            if (symbol.Name != name)
            {
                return false;
            }

            return IsMember(symbol, containingType);
        }

        private static bool IsMember(ISymbol symbol, string containingType)
        {
            return symbol.ContainingAssembly?.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.OrdinalIgnoreCase) == true &&
                   symbol.ContainingType?.ToString().Equals(containingType, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}