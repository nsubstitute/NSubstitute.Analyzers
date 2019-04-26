using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SubstituteSymbolExtensions
    {
        private static readonly IReadOnlyDictionary<string, string> ReturnsMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteReturnsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsNullMethod] = MetadataNames.NSubstituteReturnsExtensionsFullTypeName,
            [MetadataNames.NSubstituteReturnsNullForAnyArgsMethod] = MetadataNames.NSubstituteReturnsExtensionsFullTypeName
        };
        
        private static readonly IReadOnlyDictionary<string, string> ThrowsMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteThrowsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName,
            [MetadataNames.NSubstituteThrowsForAnyArgsMethod] = MetadataNames.NSubstituteExceptionExtensionsFullTypeName
        };
        
        private static readonly IReadOnlyDictionary<string, string> ReceivedMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteReceivedMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteReceivedWithAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDidNotReceiveMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteDidNotReceiveWithAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName
        };

        private static readonly IReadOnlyDictionary<string, string> WhenMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.NSubstituteWhenMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName,
            [MetadataNames.NSubstituteWhenForAnyArgsMethod] = MetadataNames.NSubstituteSubstituteExtensionsFullTypeName
        };
        
        private static readonly IReadOnlyDictionary<string, string> ArgMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.ArgIsMethodName] = MetadataNames.NSubstituteArgFullTypeName,
            [MetadataNames.ArgAnyMethodName] = MetadataNames.NSubstituteArgFullTypeName
        };
        
        private static readonly IReadOnlyDictionary<string, string> ArgCompatMethodNames = new Dictionary<string, string>
        {
            [MetadataNames.ArgIsMethodName] = MetadataNames.NSubstituteArgCompatFullTypeName,
            [MetadataNames.ArgAnyMethodName] = MetadataNames.NSubstituteArgCompatFullTypeName
        };

        public static bool IsReturnLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, ReturnsMethodNames);
        }

        public static bool IsThrowLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, ThrowsMethodNames);
        }
        
        public static bool IsReceivedLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, ReceivedMethodNames);
        }

        public static bool IsWhenLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, WhenMethodNames);
        }
        
        // TODO better name
        public static bool IsArgLikeMethod(this ISymbol symbol)
        {
            return IsMember(symbol, ArgMethodNames) || IsMember(symbol, ArgCompatMethodNames);
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