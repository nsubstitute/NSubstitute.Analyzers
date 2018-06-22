using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Newtonsoft.Json;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.Threading;

namespace NSubstitute.Analyzers.Shared.CodeFixProviders
{
    public class AbstractSuppressDiagnosticsCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticIdentifiers.NonVirtualSetupSpecification);

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var project = context.Document.Project;
            var workspace = project.Solution.Workspace;

            // check if we are allowed to add it
            if (!workspace.CanApplyChange(ApplyChangesKind.AddAdditionalDocument))
            {
                return SpecializedTasks.CompletedTask;
            }

            foreach (var diagnostic in context.Diagnostics.Where(diagnostic => FixableDiagnosticIds.Contains(diagnostic.Id)))
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Suppress in nsubstitute.json",
                        cancellationToken => GetTransformedSolutionAsync(context, diagnostic),
                        nameof(AbstractSuppressDiagnosticsCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private async Task<Solution> GetTransformedSolutionAsync(CodeFixContext context, Diagnostic diagnostic)
        {
            var project = context.Document.Project;
            var solution = project.Solution;

            var settingsFile = GetSettingsFile(project);

            // creating additional document from Roslyn is broken (https://github.com/dotnet/roslyn/issues/4655) the nsubstitute.json file have to be created by users manually
            // if there is no settings file do not provide refactorings
            if (settingsFile == null)
            {
                return solution;
            }

            var root = await context.Document.GetSyntaxRootAsync();
            var model = await context.Document.GetSemanticModelAsync();

            var syntaxNode = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var symbol = model.GetSymbolInfo(syntaxNode);

            var options = GetUpdatedAnalyzersOptions(context, diagnostic, symbol);

            project = project.RemoveAdditionalDocument(settingsFile.Id);
            solution = project.Solution;

            var newDocumentId = settingsFile.Id ?? DocumentId.CreateNewId(project.Id);

            return solution.AddAdditionalDocument(
                newDocumentId,
                AnalyzersSettings.AnalyzerFileName,
                JsonConvert.SerializeObject(options, Formatting.Indented));
        }

        private static AnalyzersSettings GetUpdatedAnalyzersOptions(CodeFixContext context, Diagnostic diagnostic, SymbolInfo symbol)
        {
            var options = context.Document.Project.AnalyzerOptions.GetSettings(default(CancellationToken));
            var target = DocumentationCommentId.CreateDeclarationId(symbol.Symbol);
            options.Suppressions = options.Suppressions ?? new List<Suppression>();

            var existingSuppression = options.Suppressions.FirstOrDefault(suppression => suppression.Target == target);

            if (existingSuppression != null)
            {
                existingSuppression.Rules = existingSuppression.Rules ?? new List<string>();
                existingSuppression.Rules.Add(diagnostic.Id);
            }
            else
            {
                options.Suppressions.Add(new Suppression
                {
                    Target = DocumentationCommentId.CreateDeclarationId(symbol.Symbol),
                    Rules = new List<string>
                    {
                        diagnostic.Id
                    }
                });
            }

            return options;
        }

        private static TextDocument GetSettingsFile(Project project)
        {
            return project.AdditionalDocuments.SingleOrDefault(document =>
                document.Name.Equals(AnalyzersSettings.AnalyzerFileName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}