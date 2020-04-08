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
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public abstract class CodeFixCodeActionsVerifier : CodeVerifier
    {
        protected CodeFixCodeActionsVerifier(WorkspaceFactory workspaceFactory)
            : base(workspaceFactory)
        {
        }

        protected abstract CodeFixProvider CodeFixProvider { get; }

        protected abstract DiagnosticAnalyzer DiagnosticAnalyzer { get; }

        protected override string AnalyzerSettings { get; } = Json.Encode(new object());

        protected async Task VerifyCodeActions(string source, params string[] expectedCodeActionTitles)
        {
            var codeActions = await RegisterCodeFixes(source);

            codeActions.Should().NotBeNull();
            codeActions.Select(action => action.Title).Should().BeEquivalentTo(expectedCodeActionTitles ?? Array.Empty<string>());
        }

        private async Task<List<CodeAction>> RegisterCodeFixes(string source)
        {
            using (var workspace = new AdhocWorkspace())
            {
                var actions = new List<CodeAction>();
                var project = AddProject(workspace.CurrentSolution, source);

                var document = project.Documents.Single();

                var compilation = await document.Project.GetCompilationAsync();
                var compilationDiagnostics = compilation.GetDiagnostics();

                VerifyNoCompilerDiagnosticErrors(compilationDiagnostics);

                var analyzerDiagnostics = await compilation.GetSortedAnalyzerDiagnostics(
                    DiagnosticAnalyzer,
                    project.AnalyzerOptions);

                foreach (var context in analyzerDiagnostics.Select(diagnostic => new CodeFixContext(
                    document,
                    analyzerDiagnostics[0],
                    (action, array) => actions.Add(action),
                    CancellationToken.None)))
                {
                    await CodeFixProvider.RegisterCodeFixesAsync(context);
                }

                return actions;
            }
        }
    }
}