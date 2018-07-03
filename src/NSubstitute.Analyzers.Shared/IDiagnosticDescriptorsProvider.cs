using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    internal interface IDiagnosticDescriptorsProvider
    {
        DiagnosticDescriptor NonVirtualSetupSpecification { get; }

        DiagnosticDescriptor UnusedReceived { get; }

        DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; }

        DiagnosticDescriptor SubstituteForPartsOfUsedForInterface { get; }

        DiagnosticDescriptor SubstituteForWithoutAccessibleConstructor { get; }

        DiagnosticDescriptor SubstituteForConstructorParametersMismatch { get; }

        DiagnosticDescriptor SubstituteForInternalMember { get; }

        DiagnosticDescriptor SubstituteConstructorMismatch { get; }

        DiagnosticDescriptor SubstituteMultipleClasses { get; }

        DiagnosticDescriptor SubstituteConstructorArgumentsForInterface { get; }

        DiagnosticDescriptor SubstituteConstructorArgumentsForDelegate { get; }

        DiagnosticDescriptor NonVirtualReceivedSetupSpecification { get; }

        DiagnosticDescriptor NonVirtualWhenSetupSpecification { get; }
    }
}