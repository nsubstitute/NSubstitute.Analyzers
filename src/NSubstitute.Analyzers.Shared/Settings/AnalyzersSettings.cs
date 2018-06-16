using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.Settings
{
    internal class AnalyzersSettings
    {
        public static string AnalyzerFileName { get; } = "nsubstitute.json";

        public static AnalyzersSettings Default => new AnalyzersSettings();

        public NonVirtualSetupSettings NonVirtualSetupSettings { get; set; }

        public AnalyzersSettings()
        {
            NonVirtualSetupSettings = new NonVirtualSetupSettings();
        }

        public AnalyzersSettings(NonVirtualSetupSettings nonVirtualSetupSettings)
        {
            NonVirtualSetupSettings = nonVirtualSetupSettings;
        }

        public static AnalyzersSettings CreateWithSuppressions(params string[] suppressions)
        {
            return new AnalyzersSettings(new NonVirtualSetupSettings
            {
                SupressedSymbols = ImmutableList.Create(suppressions)
            });
        }
    }
}