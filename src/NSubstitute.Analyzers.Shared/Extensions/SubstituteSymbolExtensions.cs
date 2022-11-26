using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class SubstituteSymbolExtensions
{
    public static bool IsAndDoesLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.NSubstituteAndDoesMethod, MetadataNames.NSubstituteConfiguredCallFullTypeName);
    }

    public static bool IsCallInfoSupportingMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.SupportingCallInfoMethodNames);
    }

    public static bool IsInitialReEntryLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.InitialReEntryMethodNames);
    }

    public static bool IsInnerReEntryLikeMethod(this ISymbol? symbol)
    {
        return IsInitialReEntryLikeMethod(symbol) ||
               IsMember(symbol, MetadataNames.NSubstituteDoMethod, MetadataNames.NSubstituteWhenCalledType);
    }

    public static bool IsReturnOrThrowLikeMethod(this ISymbol? symbol)
    {
        return IsReturnLikeMethod(symbol) || IsThrowLikeMethod(symbol);
    }

    public static bool IsReturnLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ReturnsMethodNames);
    }

    public static bool IsReturnForAnyArgsLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ReturnsForAnyArgsMethodNames);
    }

    public static bool IsThrowLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ThrowsMethodNames);
    }

    public static bool IsThrowForAnyArgsLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ThrowsForAnyArgsMethodNames);
    }

    public static bool IsThrowSyncLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ThrowsSyncMethodNames);
    }

    public static bool IsThrowsSyncMethod(this ISymbol? symbol)
    {
        return IsMember(
            symbol,
            MetadataNames.NSubstituteThrowsMethod,
            MetadataNames.NSubstituteExceptionExtensionsFullTypeName);
    }

    public static bool IsReceivedLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ReceivedMethodNames) ||
               IsMember(symbol, MetadataNames.ReceivedWithQuantityMethodNames);
    }

    public static bool IsReceivedWithAnyArgsLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ReceivedWithAnyArgsMethodNames) || IsMember(symbol, MetadataNames.ReceivedWithAnyArgsQuantityMethodNames);
    }

    public static bool IsReceivedInOrderMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.NSubstituteInOrderMethod, MetadataNames.NSubstituteReceivedFullTypeName);
    }

    public static bool IsWhenLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.WhenMethodNames);
    }

    public static bool IsWhenForAnyArgsLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.WhenForAnyArgsMethodNames);
    }

    public static bool IsArgMatcherLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ArgMatchersMethodNames) || IsMember(symbol, MetadataNames.ArgMatchersCompatMethodNames);
    }

    public static bool IsWithAnyArgsIncompatibleArgMatcherLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ArgMatchersIncompatibleWithForAnyArgsMethodNames) ||
               IsMember(symbol, MetadataNames.ArgMatchersCompatIncompatibleWithForAnyArgsMethodNames);
    }

    public static bool IsArgDoLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.ArgDoMethodName, MetadataNames.NSubstituteArgFullTypeName) ||
               IsMember(symbol, MetadataNames.ArgDoMethodName, MetadataNames.NSubstituteArgCompatFullTypeName);
    }

    public static bool IsSubstituteCreateLikeMethod(this ISymbol? symbol)
    {
        return IsMember(symbol, MetadataNames.CreateSubstituteMethodNames);
    }

    private static bool IsMember(this ISymbol? symbol, IReadOnlyDictionary<string, string> memberTypeMap)
    {
        return symbol != null && memberTypeMap.TryGetValue(symbol.Name, out var containingType) &&
               IsMember(symbol, containingType);
    }

    private static bool IsMember(ISymbol? symbol, string memberName, string containingType)
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