using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class IOperationExtensions
    {
        public static bool IsEventAssignmentOperation(this IOperation operation)
        {
            return operation is IAssignmentOperation assignmentOperation &&
                   assignmentOperation.Kind == OperationKind.EventAssignment;
        }
    }
}