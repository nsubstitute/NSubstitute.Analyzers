using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions;

internal static class IOperationExtensions
{
    public static bool IsEventAssignmentOperation(this IOperation operation)
    {
        switch (operation)
        {
            case IAssignmentOperation assignmentOperation:
                return assignmentOperation.Kind == OperationKind.EventAssignment;
            case IEventAssignmentOperation _:
                return true;
            case IExpressionStatementOperation expressionStatementOperation:
                return IsEventAssignmentOperation(expressionStatementOperation.Operation);
            default:
                return false;
        }
    }

    public static IOperation GetSubstituteOperation(this IInvocationOperation invocationOperation)
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

    public static IEnumerable<SyntaxNode> GetSyntaxes(this IArgumentOperation argumentOperation)
    {
        if (argumentOperation.Parameter.IsParams)
        {
            var initializerElementValues =
                (argumentOperation.Value as IArrayCreationOperation)?.Initializer.ElementValues;

            foreach (var operation in initializerElementValues ?? Enumerable.Empty<IOperation>())
            {
                yield return operation.Syntax;
            }

            yield break;
        }

        yield return argumentOperation.Value.Syntax;
    }

    private static bool IsImplicitlyProvidedArrayWithoutValues(IArgumentOperation arg)
    {
        return arg.IsImplicit &&
               arg.ArgumentKind == ArgumentKind.ParamArray &&
               arg.Value is IArrayCreationOperation arr &&
               !arr.Initializer.ElementValues.Any();
    }
}