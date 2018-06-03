using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    public class AbstractDiagnosticDescriptorsProvider<T> : IDiagnosticDescriptorsProvider
    {
        public DiagnosticDescriptor NonVirtualSetupSpecification { get; } = DiagnosticDescriptors<T>.NonVirtualSetupSpecification;

        public DiagnosticDescriptor UnusedReceived { get; } = DiagnosticDescriptors<T>.UnusedReceived;
    }
}