using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal sealed class NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider : AbstractNonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider
    {
        protected override ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; } = NonSubstitutableMemberArgumentMatcherAnalyzer.AllowedPaths;
    }
}