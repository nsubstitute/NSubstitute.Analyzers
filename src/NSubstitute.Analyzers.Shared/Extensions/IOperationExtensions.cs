using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<IArgumentOperation> GetOrderedArgumentOperations(this IInvocationOperation invocationOperation)
        {
            var orderedArguments = invocationOperation.Arguments
                .Where(arg => IsImplicitlyProvidedArrayWithValues(arg) == false)
                .OrderBy(arg => arg.Parameter.Ordinal);

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

        public static ITypeSymbol GetTypeSymbol(this IArgumentOperation argumentOperation)
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

        public static ITypeSymbol GetTypeSymbol(this IAssignmentOperation assignmentOperation)
        {
            ITypeSymbol conversionTypeSymbol = null;
            switch (assignmentOperation.Value)
            {
                case IConversionOperation conversionOperation:
                    conversionTypeSymbol = conversionOperation.Operand.Type;
                    break;
            }

            return conversionTypeSymbol ?? assignmentOperation.Value.Type;
        }

        public static ITypeSymbol GetArgumentOperationDeclaredTypeSymbol(this IArgumentOperation argumentOperation)
        {
            return argumentOperation.Parameter.Type;
        }

        private static bool IsImplicitlyProvidedArrayWithValues(IArgumentOperation arg)
        {
            return arg.IsImplicit &&
                   arg.ArgumentKind == ArgumentKind.ParamArray &&
                   arg.Value is IArrayCreationOperation arr &&
                   !arr.Initializer.ElementValues.Any();
        }
    }
}