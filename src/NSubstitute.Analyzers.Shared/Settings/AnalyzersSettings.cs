namespace NSubstitute.Analyzers.Shared.Settings
{
    internal class AnalyzersSettings
    {
        public static string AnalyzerFileName { get; } = "nsubstituteanalyzers.json";

        public static AnalyzersSettings Default { get; } = new AnalyzersSettings();

        public NonVirtualSetupSettings NonVirtualSetupSettings { get; set; }

        public AnalyzersSettings()
        {
            NonVirtualSetupSettings = new NonVirtualSetupSettings();
        }
    }
}