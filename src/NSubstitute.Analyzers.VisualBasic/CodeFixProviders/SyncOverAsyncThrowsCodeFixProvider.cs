using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.VisualBasic)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider
{
    public SyncOverAsyncThrowsCodeFixProvider()
        : base(SubstitutionNodeFinder.Instance)
    {
    }
}