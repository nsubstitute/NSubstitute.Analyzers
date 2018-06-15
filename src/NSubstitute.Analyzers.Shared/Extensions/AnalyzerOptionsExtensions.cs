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
            var settingsText = options.AdditionalFiles.FirstOrDefault(file =>
                Path.GetFileName(file.Path).Equals(AnalyzersSettings.AnalyzerFileName, StringComparison.CurrentCultureIgnoreCase));

            if (settingsText == null)
            {
                return AnalyzersSettings.Default;
            }

            var sourceText = settingsText.GetText(cancellationToken);

            return JsonConvert.DeserializeObject<AnalyzersSettings>(sourceText.ToString());
        }
    }
}