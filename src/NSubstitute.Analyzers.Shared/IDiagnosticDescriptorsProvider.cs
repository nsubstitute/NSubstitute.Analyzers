using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Shared
{
    internal interface IDiagnosticDescriptorsProvider
    {
        DiagnosticDescriptor NonVirtualSetupSpecification { get; }

        DiagnosticDescriptor UnusedReceived { get; }

        DiagnosticDescriptor UnusedReceivedForOrdinaryMethod { get; }
    }
}