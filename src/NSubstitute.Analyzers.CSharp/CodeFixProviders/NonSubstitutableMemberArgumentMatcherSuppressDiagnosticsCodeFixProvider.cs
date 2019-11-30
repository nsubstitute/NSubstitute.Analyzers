using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    internal sealed class NonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider
        : AbstractNonSubstitutableMemberArgumentMatcherSuppressDiagnosticsCodeFixProvider
    {
        protected override ImmutableArray<ImmutableArray<int>> AllowedAncestorPaths { get; } = NonSubstitutableMemberArgumentMatcherAnalyzer.AllowedPaths;
    }
}