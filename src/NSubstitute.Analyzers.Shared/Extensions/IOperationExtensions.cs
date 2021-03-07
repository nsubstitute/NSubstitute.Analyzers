using System;
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

        public static IEnumerable<IArgumentOperation> GetOrderedArgumentOperationsWithoutInstanceArgument(this IInvocationOperation invocationOperation)
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

        public static IEnumerable<IArgumentOperation> GetOrderedArgumentOperations(this IInvocationOperation invocationOperation)
        {
            return invocationOperation.Arguments.OrderBy(arg => arg.Parameter.Ordinal);
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

        public static IOperation GetSubstituteOperation(this IInvocationOperation invocationOperation)
        {
            if (!invocationOperation.TargetMethod.IsExtensionMethod)
            {
                return invocationOperation.Children.First();
            }

            // unlike CSharp implementation, VisualBasic doesnt include "instance" argument for reduced extensions
            if (invocationOperation.TargetMethod.MethodKind == MethodKind.ReducedExtension && invocationOperation.Language == LanguageNames.VisualBasic)
            {
                return invocationOperation.Children.First();
            }

            return GetOrderedArgumentOperations(invocationOperation).First().Value;
        }

        public static int? GetIndexerPosition(this IOperation operation)
        {
            var literal = operation switch
            {
                IArrayElementReferenceOperation arrayElementReferenceOperation => arrayElementReferenceOperation.Indices.First() as ILiteralOperation,
                IPropertyReferenceOperation propertyReferenceOperation => propertyReferenceOperation.Arguments.First().Value as ILiteralOperation,
                _ => null
            };

            if (literal == null || literal.ConstantValue.HasValue == false)
            {
                return null;
            }

            return (int)literal.ConstantValue.Value;
        }

        public static IEnumerable<SyntaxNode> GetSyntaxes(this IArgumentOperation argumentOperation)
        {
            if (argumentOperation.Parameter.IsParams)
            {
                var initializerElementValues = (argumentOperation.Value as IArrayCreationOperation)?.Initializer.ElementValues;

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
}