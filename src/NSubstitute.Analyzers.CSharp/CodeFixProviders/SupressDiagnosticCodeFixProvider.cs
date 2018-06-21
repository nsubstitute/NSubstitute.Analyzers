using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Extensions;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.Threading;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public class SupressDiagnosticCodeFixProvider : CodeFixProvider
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

            foreach (var diagnostic in context.Diagnostics)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        "Suppress in nsubstitute.json", // TODO proper message
                        cancellationToken => GetTransformedSolutionAsync(context, diagnostic),
                        nameof(SupressDiagnosticCodeFixProvider)),
                    diagnostic);
            }

            return SpecializedTasks.CompletedTask;
        }

        private async Task<Solution> GetTransformedSolutionAsync(CodeFixContext context, Diagnostic diagnostic)
        {
            var options = context.Document.Project.AnalyzerOptions.GetSettings(default(CancellationToken));
            var project = context.Document.Project;
            var solution = project.Solution;
            var root = await context.Document.GetSyntaxRootAsync().ConfigureAwait(false);
            var model = await context.Document.GetSemanticModelAsync();

            var forPartsOfNode = (InvocationExpressionSyntax)root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
            var symbol = model.GetSymbolInfo(forPartsOfNode);

            var newDocumentId = DocumentId.CreateNewId(project.Id);

            options.Suppressions.Add(new Suppression
            {
                Target = DocumentationCommentId.CreateDeclarationId(symbol.Symbol),
                Rules = new List<string>
                {
                    DiagnosticIdentifiers.NonVirtualSetupSpecification
                }
            });

            var newSolution = solution.AddAdditionalDocument(newDocumentId, AnalyzersSettings.AnalyzerFileName, JsonConvert.SerializeObject(options));

            return newSolution;
        }
    }
}