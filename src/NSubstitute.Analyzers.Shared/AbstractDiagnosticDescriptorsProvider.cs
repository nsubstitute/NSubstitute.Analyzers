using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    internal class AbstractDiagnosticDescriptorsProvider<T> : IDiagnosticDescriptorsProvider
    {
        public DiagnosticDescriptor NonVirtualSetupSpecification { get; } = DiagnosticDescriptors<T>.NonVirtualSetupSpecification;

        public DiagnosticDescriptor UnusedReceived { get; } = DiagnosticDescriptors<T>.UnusedReceived;

        public DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; } = DiagnosticDescriptors<T>.UnusedReceivedForOrdinaryMethod;

        public DiagnosticDescriptor PartialSubstituteForUnsupportedType { get; } = DiagnosticDescriptors<T>.PartialSubstituteForUnsupportedType;

        public DiagnosticDescriptor SubstituteForWithoutAccessibleConstructor { get; } = DiagnosticDescriptors<T>.SubstituteForWithoutAccessibleConstructor;

        public DiagnosticDescriptor SubstituteForConstructorParametersMismatch { get; } = DiagnosticDescriptors<T>.SubstituteForConstructorParametersMismatch;

        public DiagnosticDescriptor SubstituteForInternalMember { get; } = DiagnosticDescriptors<T>.SubstituteForInternalMember;

        public DiagnosticDescriptor SubstituteConstructorMismatch { get; } = DiagnosticDescriptors<T>.SubstituteConstructorMismatch;

        public DiagnosticDescriptor SubstituteMultipleClasses { get; } = DiagnosticDescriptors<T>.SubstituteMultipleClasses;

        public DiagnosticDescriptor SubstituteConstructorArgumentsForInterface { get; } = DiagnosticDescriptors<T>.SubstituteConstructorArgumentsForInterface;

        public DiagnosticDescriptor SubstituteConstructorArgumentsForDelegate { get; } = DiagnosticDescriptors<T>.SubstituteConstructorArgumentsForDelegate;

        public DiagnosticDescriptor NonVirtualReceivedSetupSpecification { get; } = DiagnosticDescriptors<T>.NonVirtualReceivedSetupSpecification;

        public DiagnosticDescriptor NonVirtualWhenSetupSpecification { get; } = DiagnosticDescriptors<T>.NonVirtualWhenSetupSpecification;

        public DiagnosticDescriptor ReEntrantSubstituteCall { get; } = DiagnosticDescriptors<T>.ReEntrantSubstituteCall;

        public DiagnosticDescriptor CallInfoArgumentOutOfRange { get; } = DiagnosticDescriptors<T>.CallInfoArgumentOutOfRange;

        public DiagnosticDescriptor CallInfoCouldNotConvertParameterAtPosition { get; } = DiagnosticDescriptors<T>.CallInfoCouldNotConvertParameterAtPosition;

        public DiagnosticDescriptor CallInfoCouldNotFindArgumentToThisCall { get; } = DiagnosticDescriptors<T>.CallInfoCouldNotFindArgumentToThisCall;

        public DiagnosticDescriptor CallInfoMoreThanOneArgumentOfType { get; } = DiagnosticDescriptors<T>.CallInfoMoreThanOneArgumentOfType;

        public DiagnosticDescriptor CallInfoArgumentSetWithIncompatibleValue { get; } = DiagnosticDescriptors<T>.CallInfoArgumentSetWithIncompatibleValue;

        public DiagnosticDescriptor CallInfoArgumentIsNotOutOrRef { get; } = DiagnosticDescriptors<T>.CallInfoArgumentIsNotOutOrRef;

        public DiagnosticDescriptor ArgumentMatcherUsedOutsideOfCall { get; } = DiagnosticDescriptors<T>.ArgumentMatcherUsedOutsideOfCall;
    }
}