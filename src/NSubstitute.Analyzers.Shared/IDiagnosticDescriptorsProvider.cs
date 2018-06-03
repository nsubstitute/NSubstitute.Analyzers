using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    public interface IDiagnosticDescriptorsProvider
    {
        DiagnosticDescriptor NonVirtualSetupSpecification { get; }

        DiagnosticDescriptor UnusedReceived { get; }
    }
}