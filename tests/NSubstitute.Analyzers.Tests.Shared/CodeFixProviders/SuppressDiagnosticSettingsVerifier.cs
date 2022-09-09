using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

public abstract class SuppressDiagnosticSettingsVerifier : CodeFixVerifier
{
    protected SuppressDiagnosticSettingsVerifier(WorkspaceFactory workspaceFactory)
        : base(workspaceFactory)
    {
    }

    protected override string AnalyzerSettings { get; } = Json.Encode(new object());

    protected async Task VerifySuppressionSettings(string source, string target, string diagnosticRuleId, int codeFixIndex = 0)
    {
        var originalSettings = Json.Decode<AnalyzersSettings>(AnalyzerSettings);

        var document = await ApplySettingsSuppressionFix(source, codeFixIndex);

        await VerifySuppressionSettings(document, originalSettings, target, diagnosticRuleId);
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

    private async Task<Document> ApplySettingsSuppressionFix(string source, int? codeFixIndex = null)
    {
        using var workspace = new AdhocWorkspace();
        var project = AddProject(workspace.CurrentSolution, source);

        var document = project.Documents.Single();
        var compilation = await document.Project.GetCompilationAsync();
        var compilerDiagnostics = compilation.GetDiagnostics();

        VerifyNoCompilerDiagnosticErrors(compilerDiagnostics);

        var analyzerDiagnostics = await compilation.GetSortedAnalyzerDiagnostics(
            DiagnosticAnalyzer,
            project.AnalyzerOptions);

        var actions = new List<CodeAction>();
        var context = new CodeFixContext(
            document,
            analyzerDiagnostics.Single(),
            (a, d) => actions.Add(a),
            CancellationToken.None);

        await CodeFixProvider.RegisterCodeFixesAsync(context);
        var action = actions[codeFixIndex ?? 0];

        return await document.ApplyCodeAction(action);
    }

    private static AnalyzersSettings GetExpectedSettings(AnalyzersSettings originalSettings, string target, string diagnosticRuleId)
    {
        var originalSupressions = originalSettings?.Suppressions ?? new List<Suppression>();
        var targetSuppression = originalSupressions.SingleOrDefault(suppression => suppression.Target == target);
        if (targetSuppression != null)
        {
            targetSuppression.Rules ??= new List<string>();
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