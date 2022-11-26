using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.VisualBasic)]
internal sealed class SyncOverAsyncThrowsAnalyzer : AbstractSyncOverAsyncThrowsAnalyzer
{
    public SyncOverAsyncThrowsAnalyzer()
        : base(VisualBasic.DiagnosticDescriptorsProvider.Instance, SubstitutionOperationFinder.Instance)
    {
    }
}