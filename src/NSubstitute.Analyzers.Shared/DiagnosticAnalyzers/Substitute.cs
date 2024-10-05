using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using NSubstitute.Analyzers.Shared.Extensions;

namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

internal class Substitute
{
    public IReadOnlyList<CallArgument> CallArguments { get; }

    private Substitute(
        IInvocationOperation? substituteInvocation,
        IPropertyReferenceOperation? substitutePropertyReference,
        IInvocationOperation configuredCall)
    {
        var argumentOperations = substituteInvocation?.Arguments ?? substitutePropertyReference?.Arguments ??
            throw new ArgumentNullException(nameof(substituteInvocation), $"Both {nameof(substituteInvocation)} and {nameof(substitutePropertyReference)} are null");

        var callArguments = argumentOperations.OrderBy(argOperation => argOperation.Parameter.Ordinal)
            .Select(argOperation => new CallArgument(argOperation))
            .ToList();

        this.CallArguments = callArguments;

        if (substitutePropertyReference is null)
        {
           return;
        }

        if (HasAssignment(substitutePropertyReference) && configuredCall.TargetMethod.IsDoLikeMethod())
        {
            callArguments.Add(new CallArgument(
                substitutePropertyReference.Property.Type,
                substitutePropertyReference.Property.Type));
        }
    }

    public static Substitute? TryCreate(IOperation substitutedOperation, IInvocationOperation configuredCall)
    {
        var actualOperation = GetActualOperation(substitutedOperation);

        if (actualOperation == null)
        {
            return null;
        }

        return new Substitute(
            actualOperation.Value.InvocationOperation,
            actualOperation.Value.PropertyReferenceOperation,
            configuredCall);
    }

    public bool HasCallArgumentAt(int position) => position >= 0 && position <= this.CallArguments.Count - 1;

    public CallArgument GetCallArgumentAt(int position)
    {
        if (position <= this.CallArguments.Count - 1)
        {
            return this.CallArguments[position];
        }

        throw new ArgumentException($"Could not find call argument at position {position}", nameof(position));
    }

    private static (IPropertyReferenceOperation? PropertyReferenceOperation, IInvocationOperation? InvocationOperation)? GetActualOperation(IOperation substituteOperation)
    {
        return substituteOperation switch
        {
            IInvocationOperation invocationOperation => (null, invocationOperation),
            IPropertyReferenceOperation propertyReferenceOperation => (PropertyReferenceOperation: propertyReferenceOperation, null),
            IConversionOperation conversionOperation => GetActualOperation(conversionOperation.Operand),
            _ => null
        };
    }

    private static bool HasAssignment(IPropertyReferenceOperation substitutePropertyReference)
    {
        return substitutePropertyReference.Property.SetMethod != null &&
               (substitutePropertyReference.Parent is IAssignmentOperation ||
                (substitutePropertyReference.Parent.Language == LanguageNames.VisualBasic &&
                 substitutePropertyReference.Parent is IBinaryOperation { OperatorKind: BinaryOperatorKind.Equals }));
    }
}