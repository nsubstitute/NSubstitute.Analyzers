using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders;

internal class AbstractNonSubstitutableMemberSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
        DiagnosticIdentifiers.NonVirtualSetupSpecification,
        DiagnosticIdentifiers.NonVirtualReceivedInOrderSetupSpecification);
}