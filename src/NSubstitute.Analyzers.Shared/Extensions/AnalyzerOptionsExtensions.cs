using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;
using Newtonsoft.Json;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class AnalyzerOptionsExtensions
    {
        public static AnalyzersSettings GetSettings(this AnalyzerOptions options, CancellationToken cancellationToken)
        {
            var settingsText = options.AdditionalFiles.FirstOrDefault(additionalText => Path.GetFileName(additionalText.Path).Equals(AnalyzersSettings.AnalyzerFileName, StringComparison.CurrentCultureIgnoreCase));

            if (settingsText == null)
            {
                return AnalyzersSettings.Default;
            }

            try
            {
                var sourceText = settingsText.GetText(cancellationToken);

                return JsonConvert.DeserializeObject<AnalyzersSettings>(sourceText.ToString());
            }
            catch (Exception)
            {
                return AnalyzersSettings.Default;
            }
        }
    }
}