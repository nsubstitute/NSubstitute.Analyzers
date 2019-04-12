using System.Collections.Immutable;
using System.Threading;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared.ExtensionsTests
{
    public class AnalyzerOptionsExtensionsTests
    {
        [Fact]
        public void GetSettings_ReturnsDefaultSettings_WhenAnalyzersSettingsFileDoesNotExist()
        {
            var analyzerAdditionalTexts =
                ImmutableArray.Create<AdditionalText>(new AnalyzerAdditionalText("othername", string.Empty));

            var analyzerOptions = new AnalyzerOptions(analyzerAdditionalTexts);

            var settings = analyzerOptions.GetSettings(CancellationToken.None);

            settings.Should().BeEquivalentTo(AnalyzersSettings.Default);
        }

        [Theory]
        [InlineData("nsubstitute.json")]
        [InlineData("Nsubstitute.json")]
        [InlineData("NsubStitute.Json")]
        public void GetSettings_ReturnsSerializedSettings_WhenAnalyzerFileExists(string fileName)
        {
            var analyzersSettings = AnalyzersSettings.CreateWithSuppressions("supression", "NS001");
            var fileContent = Json.Encode(analyzersSettings);
            var analyzerAdditionalTexts =
                ImmutableArray.Create<AdditionalText>(new AnalyzerAdditionalText(fileName, fileContent));

            var analyzerOptions = new AnalyzerOptions(analyzerAdditionalTexts);
            var settings = analyzerOptions.GetSettings(CancellationToken.None);

            settings.Should().BeEquivalentTo(analyzersSettings);
        }

        [Fact]
        public void GetSettings_ReturnsDefaultSettings_WhenAnalyzersSettingsFileHasIncorrectFormat()
        {
            var analyzerAdditionalTexts =
                ImmutableArray.Create<AdditionalText>(new AnalyzerAdditionalText(AnalyzersSettings.AnalyzerFileName, "{["));

            var analyzerOptions = new AnalyzerOptions(analyzerAdditionalTexts);

            var settings = analyzerOptions.GetSettings(CancellationToken.None);

            settings.Should().BeEquivalentTo(AnalyzersSettings.Default);
        }
    }
}