using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions
{
    public static class ProjectExtensions
    {
        public static async Task<ImmutableArray<Diagnostic>> GetSortedAnalyzerDiagnostics(
            this Project project,
            DiagnosticAnalyzer analyzer,
            CancellationToken cancellationToken = default)
        {
            var compilation = await project.GetCompilationAsync(cancellationToken);
            return await compilation.GetSortedAnalyzerDiagnostics(analyzer, project.AnalyzerOptions, cancellationToken);
        }

        public static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnostics(
            this Project project,
            DiagnosticAnalyzer analyzer,
            IComparer<Diagnostic> comparer = null,
            CancellationToken cancellationToken = default)
        {
            var compilation = await project.GetCompilationAsync(cancellationToken);
            return await compilation.GetAnalyzerDiagnostics(
                analyzer,
                project.AnalyzerOptions,
                comparer,
                cancellationToken);
        }
    }
}