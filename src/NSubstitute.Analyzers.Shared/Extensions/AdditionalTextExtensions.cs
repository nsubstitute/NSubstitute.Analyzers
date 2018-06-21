using System;
using System.IO;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    public static class AdditionalTextExtensions
    {
        public static bool IsSettingsFile(this AdditionalText additionalText)
        {
            return Path.GetFileName(additionalText.Path).Equals(AnalyzersSettings.AnalyzerFileName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}