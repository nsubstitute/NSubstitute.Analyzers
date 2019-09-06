using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class IArgumentOperationExtensions
    {
        public static ITypeSymbol GetArgumentOperationActualTypeSymbol(this IArgumentOperation argumentOperation)
        {
            ITypeSymbol conversionTypeSymbol = null;
            switch (argumentOperation.Value)
            {
                case IConversionOperation conversionOperation:
                    conversionTypeSymbol = conversionOperation.Operand.Type;
                    break;
            }

            return conversionTypeSymbol ?? argumentOperation.GetArgumentOperationDeclaredTypeSymbol();
        }

        public static ITypeSymbol GetArgumentOperationDeclaredTypeSymbol(this IArgumentOperation argumentOperation)
        {
            return argumentOperation.Parameter.Type;
        }
    }
}