using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared;

internal interface IDiagnosticDescriptorsProvider
{
    DiagnosticDescriptor NonVirtualSetupSpecification { get; }

    DiagnosticDescriptor NonVirtualReceivedSetupSpecification { get; }

    DiagnosticDescriptor NonVirtualReceivedInOrderSetupSpecification { get; }

    DiagnosticDescriptor InternalSetupSpecification { get; }

    DiagnosticDescriptor UnusedReceived { get; }

    DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; }

    DiagnosticDescriptor PartialSubstituteForUnsupportedType { get; }

    DiagnosticDescriptor SubstituteForWithoutAccessibleConstructor { get; }

    DiagnosticDescriptor SubstituteForConstructorParametersMismatch { get; }

    DiagnosticDescriptor SubstituteForInternalMember { get; }

    DiagnosticDescriptor SubstituteConstructorMismatch { get; }

    DiagnosticDescriptor SubstituteMultipleClasses { get; }

    DiagnosticDescriptor SubstituteConstructorArgumentsForInterface { get; }

    DiagnosticDescriptor SubstituteConstructorArgumentsForDelegate { get; }

    DiagnosticDescriptor ReEntrantSubstituteCall { get; }

    DiagnosticDescriptor CallInfoArgumentOutOfRange { get; }

    DiagnosticDescriptor CallInfoCouldNotConvertParameterAtPosition { get; }

    DiagnosticDescriptor CallInfoCouldNotFindArgumentToThisCall { get; }

    DiagnosticDescriptor CallInfoMoreThanOneArgumentOfType { get; }

    DiagnosticDescriptor CallInfoArgumentSetWithIncompatibleValue { get; }

    DiagnosticDescriptor CallInfoArgumentIsNotOutOrRef { get; }

    DiagnosticDescriptor ConflictingArgumentAssignments { get; }

    DiagnosticDescriptor NonSubstitutableMemberArgumentMatcherUsage { get; }

    DiagnosticDescriptor WithAnyArgsArgumentMatcherUsage { get; }

    DiagnosticDescriptor ReceivedUsedInReceivedInOrder { get; }

    DiagnosticDescriptor AsyncCallbackUsedInReceivedInOrder { get; }

    DiagnosticDescriptor SyncOverAsyncThrows { get; }
}