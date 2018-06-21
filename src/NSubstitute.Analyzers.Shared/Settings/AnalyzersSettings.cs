using System.Collections.Generic;
using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Shared.Settings
{
    internal class AnalyzersSettings
    {
        public static string AnalyzerFileName { get; } = "nsubstitute.json";

        public static AnalyzersSettings Default => new AnalyzersSettings();

        public List<Suppression> Suppressions { get; set; }

        public AnalyzersSettings()
        {
            Suppressions = new List<Suppression>();
        }

        public AnalyzersSettings(List<Suppression> suppressions)
        {
            Suppressions = suppressions;
        }

        public static AnalyzersSettings CreateWithSuppressions(params string[] suppressions)
        {
            return new AnalyzersSettings(new List<Suppression>());
        }
    }
}