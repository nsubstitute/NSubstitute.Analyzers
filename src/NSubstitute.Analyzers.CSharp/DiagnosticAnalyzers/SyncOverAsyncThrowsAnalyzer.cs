using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class SyncOverAsyncThrowsAnalyzer : AbstractSyncOverAsyncThrowsAnalyzer
{
    public SyncOverAsyncThrowsAnalyzer()
        : base(CSharp.DiagnosticDescriptorsProvider.Instance, SubstitutionNodeFinder.Instance)
    {
    }
}