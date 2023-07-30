using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class IOperationExtensions
{
    public static bool IsEventAssignmentOperation(this IOperation operation)
    {
        return operation switch
        {
            IAssignmentOperation assignmentOperation => assignmentOperation.Kind == OperationKind.EventAssignment,
            IEventAssignmentOperation _ => true,
            IExpressionStatementOperation expressionStatementOperation => IsEventAssignmentOperation(
                expressionStatementOperation.Operation),
            _ => false
        };
    }

    public static IOperation GetSubstituteOperation(this IPropertyReferenceOperation propertyReferenceOperation) =>
        propertyReferenceOperation.Instance;

    public static IOperation? GetSubstituteOperation(this IInvocationOperation invocationOperation)
    {
        if (invocationOperation.Instance != null)
        {
            return invocationOperation.Instance;
        }

        return invocationOperation.Arguments.FirstOrDefault(arg => arg.Parameter.Ordinal == 0)?.Value;
    }

    public static IEnumerable<IArgumentOperation> GetOrderedArgumentOperations(
        this IInvocationOperation invocationOperation)
    {
        return invocationOperation.Arguments.OrderBy(arg => arg.Parameter.Ordinal);
    }

    public static IEnumerable<IArgumentOperation> GetOrderedArgumentOperationsWithoutInstanceArgument(
        this IInvocationOperation invocationOperation)
    {
        var orderedArguments = invocationOperation.GetOrderedArgumentOperations()
            .Where(arg => IsImplicitlyProvidedArrayWithoutValues(arg) == false);

        if (!invocationOperation.TargetMethod.IsExtensionMethod)
        {
            return orderedArguments;
        }

        // unlike CSharp implementation, VisualBasic doesnt include "instance" argument for reduced extensions
        if (invocationOperation.TargetMethod.MethodKind == MethodKind.ReducedExtension &&
            invocationOperation.Language == LanguageNames.VisualBasic)
        {
            return orderedArguments;
        }

        return orderedArguments.Skip(1);
    }

    public static int? GetIndexerPosition(this IOperation operation)
    {
        var literal = operation switch
        {
            IArrayElementReferenceOperation arrayElementReferenceOperation => arrayElementReferenceOperation.Indices
                .First() as ILiteralOperation,
            IPropertyReferenceOperation propertyReferenceOperation =>
                propertyReferenceOperation.Arguments.First().Value as ILiteralOperation,
            IInvocationOperation invocationOperation =>
                invocationOperation.Arguments.First().Value as ILiteralOperation,
            _ => null
        };

        if (literal == null || literal.ConstantValue.HasValue == false)
        {
            return null;
        }

        return (int)literal.ConstantValue.Value;
    }

    public static ITypeSymbol GetTypeSymbol(this IArgumentOperation argumentOperation)
    {
        var conversionTypeSymbol = argumentOperation.Value switch
        {
            IConversionOperation conversionOperation => conversionOperation.Operand.Type,
            _ => null
        };

        return conversionTypeSymbol ?? argumentOperation.GetArgumentOperationDeclaredTypeSymbol();
    }

    public static ITypeSymbol? GetTypeSymbol(this IAssignmentOperation assignmentOperation)
    {
        return assignmentOperation.Value switch
        {
            IConversionOperation conversionOperation => conversionOperation.Operand.Type,
            _ => assignmentOperation.Value.Type
        };
    }

    public static IEnumerable<IOperation> Ancestors(this IOperation operation)
    {
        var parent = operation.Parent;
        while (parent != null)
        {
            yield return parent;
            parent = parent.Parent;
        }
    }

    public static ISymbol? ExtractSymbol(this IOperation? operation)
    {
        var symbol = operation switch
        {
            IMemberReferenceOperation memberReferenceOperation => memberReferenceOperation.Member,
            IInvocationOperation invocationOperation => invocationOperation.TargetMethod,
            IConversionOperation conversionOperation => ExtractSymbol(conversionOperation.Operand),
            IAwaitOperation awaitOperation => ExtractSymbol(awaitOperation.Operation),
            ILocalReferenceOperation localReferenceOperation => localReferenceOperation.Local,
            _ => null
        };

        return symbol;
    }

    public static IEnumerable<IOperation>? GetArrayElementValues(this IOperation operation)
    {
        return operation switch
        {
            IArrayCreationOperation arrayCreationOperation => arrayCreationOperation.Initializer.ElementValues,
            _ => null
        };
    }

    private static bool IsImplicitlyProvidedArrayWithoutValues(IArgumentOperation arg)
    {
        return arg.IsImplicit &&
               arg.ArgumentKind == ArgumentKind.ParamArray &&
               arg.Value is IArrayCreationOperation arr &&
               !arr.Initializer.ElementValues.Any();
    }
}