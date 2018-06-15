using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.Settings
{
    internal class NonVirtualSetupSettings
    {
        public ImmutableList<string> SupressedSymbols { get; set; } = ImmutableList<string>.Empty;
    }
}