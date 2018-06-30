using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    internal class AbstractDiagnosticDescriptorsProvider<T> : IDiagnosticDescriptorsProvider
    {
        public DiagnosticDescriptor NonVirtualSetupSpecification { get; } = DiagnosticDescriptors<T>.NonVirtualSetupSpecification;

        public DiagnosticDescriptor UnusedReceived { get; } = DiagnosticDescriptors<T>.UnusedReceived;

        public DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; } = DiagnosticDescriptors<T>.UnusedReceivedForOrdinaryMethod;

        public DiagnosticDescriptor SubstituteForPartsOfUsedForInterface { get; } = DiagnosticDescriptors<T>.SubstituteForPartsOfUsedForInterface;

        public DiagnosticDescriptor SubstituteForWithoutAccessibleConstructor { get; } = DiagnosticDescriptors<T>.SubstituteForWithoutAccessibleConstructor;

        public DiagnosticDescriptor SubstituteForConstructorParametersMismatch { get; } = DiagnosticDescriptors<T>.SubstituteForConstructorParametersMismatch;

        public DiagnosticDescriptor SubstituteForInternalMember { get; } = DiagnosticDescriptors<T>.SubstituteForInternalMember;

        public DiagnosticDescriptor SubstituteConstructorMismatch { get; } = DiagnosticDescriptors<T>.SubstituteConstructorMismatch;

        public DiagnosticDescriptor SubstituteMultipleClasses { get; } = DiagnosticDescriptors<T>.SubstituteMultipleClasses;

        public DiagnosticDescriptor SubstituteConstructorArgumentsForInterface { get; } = DiagnosticDescriptors<T>.SubstituteConstructorArgumentsForInterface;

        public DiagnosticDescriptor SubstituteConstructorArgumentsForDelegate { get; } = DiagnosticDescriptors<T>.SubstituteConstructorArgumentsForDelegate;

        public DiagnosticDescriptor NonVirtualWhenSetupSpecification { get; } = DiagnosticDescriptors<T>.NonVirtualWhenSetupSpecification;
    }
}