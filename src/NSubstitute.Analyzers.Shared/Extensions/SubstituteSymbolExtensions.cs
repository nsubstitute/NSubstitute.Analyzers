using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SubstituteSymbolExtensions
    {
        public static bool IsAndDoesLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.NSubstituteAndDoesMethod, MetadataNames.NSubstituteConfiguredCallFullTypeName);
        }

        public static bool IsCallInfoSupportingMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.SupportingCallInfoMethodNames);
        }

        public static bool IsInitialReEntryLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.InitialReEntryMethodNames);
        }

        public static bool IsInnerReEntryLikeMethod(this ISymbol symbol)
        {
            return IsInitialReEntryLikeMethod(symbol) ||
                   IsMember(symbol, MetadataNames.NSubstituteDoMethod, MetadataNames.NSubstituteWhenCalledType);
        }

        public static bool IsReturnOrThrowLikeMethod(this ISymbol symbol)
        {
            return IsReturnLikeMethod(symbol) || IsThrowLikeMethod(symbol);
        }

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

        public static bool IsSubstituteCreateLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, MetadataNames.CreateSubstituteMethodNames);
        }

        private static bool IsMember(this ISymbol symbol, IReadOnlyDictionary<string, string> memberTypeMap)
        {
            if (symbol == null)
            {
                return false;
            }

            return memberTypeMap.TryGetValue(symbol.Name, out var containingType) && IsMember(symbol, containingType);
        }

        private static bool IsMember(ISymbol symbol, string memberName, string containingType)
        {
            return symbol != null && symbol.Name == memberName && IsMember(symbol, containingType);
        }

        private static bool IsMember(ISymbol symbol, string containingType)
        {
            var containingAssembly = symbol.ContainingAssembly;
            var symbolContainingType = symbol.ContainingType;

            if (containingAssembly == null || symbolContainingType == null)
            {
                return false;
            }

            return containingAssembly.Name.Equals(MetadataNames.NSubstituteAssemblyName, StringComparison.Ordinal) &&
                   (symbolContainingType.ToString().Equals(containingType, StringComparison.Ordinal) ||
                    symbolContainingType.ConstructedFrom?.Name.Equals(containingType, StringComparison.Ordinal) == true);
        }
    }
}