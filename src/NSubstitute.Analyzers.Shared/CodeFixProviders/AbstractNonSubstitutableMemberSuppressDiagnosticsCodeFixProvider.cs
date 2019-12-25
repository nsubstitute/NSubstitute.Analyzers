using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    internal class AbstractNonSubstitutableMemberSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.NonVirtualSetupSpecification);
    }
}