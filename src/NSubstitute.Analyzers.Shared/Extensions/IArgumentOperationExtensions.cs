using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class IArgumentOperationExtensions
{
    public static ITypeSymbol GetArgumentOperationDeclaredTypeSymbol(this IArgumentOperation argumentOperation)
    {
        return argumentOperation.Parameter.Type;
    }
}