using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public abstract class SuppressDiagnosticSettingsVerifier : CodeFixVerifier
    {
        protected async Task VerifySuppressionSettings(string source, string target, string diagnosticRuleId, int codeFixIndex = 0)
        {
            var originalSettings = Json.Decode<AnalyzersSettings>(GetSettings());

            var document = await ApplySettingsSuppressionFix(source, codeFixIndex);

            await VerifySuppressionSettings(document, originalSettings, target, diagnosticRuleId);
        }

        protected override string GetSettings()
        {
            return Json.Encode(new object());
        }

        private async Task VerifySuppressionSettings(Document document, AnalyzersSettings originalSettings, string target, string diagnosticRuleId)
        {
            var textDocument = document.Project
                .AdditionalDocuments
                .Single(additionalDocument => additionalDocument.Name.Equals(
                    AnalyzersSettings.AnalyzerFileName,
                    StringComparison.CurrentCultureIgnoreCase));

            var text = await textDocument.GetTextAsync();

            var updatedSettings = Json.Decode<AnalyzersSettings>(text.ToString());
            var expectedSettings = GetExpectedSettings(originalSettings, target, diagnosticRuleId);

            updatedSettings.Should().BeEquivalentTo(expectedSettings);
        }

        private async Task<Document> ApplySettingsSuppressionFix(string oldSource, int? codeFixIndex = null)
        {
            return await ApplySettingsSuppressionFix(Language, GetDiagnosticAnalyzer(), GetCodeFixProvider(), oldSource, codeFixIndex);
        }

        private async Task<Document> ApplySettingsSuppressionFix(string language, DiagnosticAnalyzer analyzer, CodeFixProvider codeFixProvider, string oldSource, int? codeFixIndex = null)
        {
            var document = CreateDocument(oldSource, language);
            var analyzerDiagnostics = await GetSortedDiagnosticsFromDocuments(analyzer, new[] { document }, false);
            var attempts = analyzerDiagnostics.Length;

            for (var i = 0; i < attempts; ++i)
            {
                var actions = new List<CodeAction>();
                var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, d) => actions.Add(a), CancellationToken.None);
                await codeFixProvider.RegisterCodeFixesAsync(context);

                if (!actions.Any())
                {
                    break;
                }

                if (codeFixIndex != null)
                {
                    document = await ApplyFix(document, actions.ElementAt((int)codeFixIndex));
                    break;
                }

                document = await ApplyFix(document, actions.ElementAt(0));
            }

            return document;
        }

        private static AnalyzersSettings GetExpectedSettings(AnalyzersSettings originalSettings, string target, string diagnosticRuleId)
        {
            var originalSupressions = originalSettings?.Suppressions ?? new List<Suppression>();
            var targetSuppression = originalSupressions.SingleOrDefault(suppression => suppression.Target == target);
            if (targetSuppression != null)
            {
                targetSuppression.Rules = targetSuppression.Rules ?? new List<string>();
                targetSuppression.Rules.Add(diagnosticRuleId);
            }
            else
            {
                originalSupressions.Add(new Suppression
                {
                    Target = target,
                    Rules = new List<string>
                    {
                        diagnosticRuleId
                    }
                });
            }

            return new AnalyzersSettings(originalSupressions);
        }
    }
}