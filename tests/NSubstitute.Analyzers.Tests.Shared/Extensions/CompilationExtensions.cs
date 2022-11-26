using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions;

public static class CompilationExtensions
{
    public static Task<ImmutableArray<Diagnostic>> GetSortedAnalyzerDiagnostics(
        this Compilation compilation,
        DiagnosticAnalyzer analyzer,
        AnalyzerOptions analyzerOptions,
        CancellationToken cancellationToken = default)
    {
        return compilation.GetAnalyzerDiagnostics(
            analyzer,
            analyzerOptions,
            DiagnosticComparer.Span,
            cancellationToken);
    }

    public static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnostics(
        this Compilation compilation,
        DiagnosticAnalyzer analyzer,
        AnalyzerOptions analyzerOptions,
        IComparer<Diagnostic>? comparer = null,
        CancellationToken cancellationToken = default)
    {
        var compilationWithAnalyzers = compilation.WithAnalyzers(ImmutableArray.Create(analyzer), analyzerOptions, cancellationToken);

        var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(cancellationToken);

        if (comparer != null)
        {
            diagnostics = diagnostics.Sort(comparer);
        }

        return diagnostics;
    }
}