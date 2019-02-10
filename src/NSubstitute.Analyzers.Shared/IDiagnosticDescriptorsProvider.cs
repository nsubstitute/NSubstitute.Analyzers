using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    internal interface IDiagnosticDescriptorsProvider
    {
        DiagnosticDescriptor NonVirtualSetupSpecification { get; }

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

        DiagnosticDescriptor NonVirtualReceivedSetupSpecification { get; }

        DiagnosticDescriptor InternalReceivedSetupSpecification { get; }

        DiagnosticDescriptor NonVirtualWhenSetupSpecification { get; }

        DiagnosticDescriptor ReEntrantSubstituteCall { get; }

        DiagnosticDescriptor CallInfoArgumentOutOfRange { get; }

        DiagnosticDescriptor CallInfoCouldNotConvertParameterAtPosition { get; }

        DiagnosticDescriptor CallInfoCouldNotFindArgumentToThisCall { get; }

        DiagnosticDescriptor CallInfoMoreThanOneArgumentOfType { get; }

        DiagnosticDescriptor CallInfoArgumentSetWithIncompatibleValue { get; }

        DiagnosticDescriptor CallInfoArgumentIsNotOutOrRef { get; }
    }
}