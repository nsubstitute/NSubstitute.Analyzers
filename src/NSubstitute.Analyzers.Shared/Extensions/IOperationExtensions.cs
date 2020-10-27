using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace NSubstitute.Analyzers.Shared.Extensions
{
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
                default:
                    return false;
            }
        }
    }
}