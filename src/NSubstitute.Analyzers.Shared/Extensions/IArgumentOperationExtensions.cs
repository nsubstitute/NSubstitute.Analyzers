using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class IArgumentOperationExtensions
    {
        public static ITypeSymbol GetArgumentOperationTypeSymbol(this IArgumentOperation argumentOperation)
        {
            ITypeSymbol conversionTypeSymbol = null;
            switch (argumentOperation.Value)
            {
                case IConversionOperation conversionOperation:
                    conversionTypeSymbol = conversionOperation.Operand.Type;
                    break;
            }

            return conversionTypeSymbol ?? argumentOperation.Parameter.Type;
        }
    }
}