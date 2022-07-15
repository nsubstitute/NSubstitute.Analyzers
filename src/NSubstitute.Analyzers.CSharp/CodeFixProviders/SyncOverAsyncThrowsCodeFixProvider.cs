using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.CodeFixProviders;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders;

[ExportCodeFixProvider(LanguageNames.CSharp)]
internal sealed class SyncOverAsyncThrowsCodeFixProvider : AbstractSyncOverAsyncThrowsCodeFixProvider
{
    public SyncOverAsyncThrowsCodeFixProvider()
        : base(SubstitutionNodeFinder.Instance)
    {
    }
}