using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.Extensions;

namespace NSubstitute.Analyzers.Tests.Shared.CodeFixProviders
{
    public abstract class CodeFixVerifier : CodeVerifier
    {
        protected CodeFixVerifier(WorkspaceFactory workspaceFactory)
            : base(workspaceFactory)
        {
        }

        protected abstract CodeFixProvider CodeFixProvider { get; }

        protected abstract DiagnosticAnalyzer DiagnosticAnalyzer { get; }

        protected async Task VerifyFix(string oldSource, string newSource, int? codeFixIndex = null)
        {
            using (var workspace = new AdhocWorkspace())
            {
                var project = AddProject(workspace.CurrentSolution, oldSource);
                var document = project.Documents.Single();
                var compilation = await project.GetCompilationAsync();

                var compilerDiagnostics = compilation.GetDiagnostics();

                VerifyNoCompilerDiagnosticErrors(compilerDiagnostics);

                var analyzerDiagnostics = await compilation.GetSortedAnalyzerDiagnostics(
                    DiagnosticAnalyzer,
                    project.AnalyzerOptions);

                var previousAnalyzerDiagnostics = analyzerDiagnostics;
                var attempts = analyzerDiagnostics.Length;

                for (var i = 0; i < attempts; ++i)
                {
                    var actions = new List<CodeAction>();
                    var context = new CodeFixContext(document, analyzerDiagnostics[0], (a, d) => actions.Add(a), CancellationToken.None);
                    await CodeFixProvider.RegisterCodeFixesAsync(context);

                    if (!actions.Any())
                    {
                        break;
                    }

                    document = await document.ApplyCodeAction(actions[codeFixIndex ?? 0]);
                    compilation = await document.Project.GetCompilationAsync();

                    compilerDiagnostics = compilation.GetDiagnostics();

                    VerifyNoCompilerDiagnosticErrors(compilerDiagnostics);

                    analyzerDiagnostics = await compilation.GetSortedAnalyzerDiagnostics(
                        DiagnosticAnalyzer,
                        project.AnalyzerOptions);

                    // check if there are analyzer diagnostics left after the code fix
                    var newAnalyzerDiagnostics = analyzerDiagnostics.Except(previousAnalyzerDiagnostics).ToList();
                    if (analyzerDiagnostics.Length == previousAnalyzerDiagnostics.Length && newAnalyzerDiagnostics.Any())
                    {
                        Execute.Assertion.Fail(
                            $"Fix didn't fix analyzer diagnostics: {newAnalyzerDiagnostics.ToDebugString()} New document:{Environment.NewLine}{await document.ToFullString()}");
                    }

                    previousAnalyzerDiagnostics = analyzerDiagnostics;
                }

                var actual = await document.ToFullString();

                actual.Should().Be(newSource);
            }
        }
    }
}
