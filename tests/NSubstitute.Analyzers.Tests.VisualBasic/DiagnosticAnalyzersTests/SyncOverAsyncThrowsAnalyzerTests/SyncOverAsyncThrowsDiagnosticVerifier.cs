using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SyncOverAsyncThrowsAnalyzerTests;

public abstract class SyncOverAsyncThrowsDiagnosticVerifier : VisualBasicDiagnosticVerifier, ISyncOverAsyncThrowsDiagnosticVerifier
{
    public static IEnumerable<object[]> ThrowsTestCases
    {
        get
        {
            yield return new object[] { "Throws" };
            yield return new object[] { "ThrowsForAnyArgs" };
        }
    }

    protected DiagnosticDescriptor SyncOverAsyncThrowsDescriptor => DiagnosticDescriptors<DiagnosticDescriptorsProvider>.SyncOverAsyncThrows;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SyncOverAsyncThrowsAnalyzer();

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReportsDiagnostic_WhenUsedInTaskReturningMethod(string method);

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReportsDiagnostic_WhenUsedInTaskReturningProperty(string method);

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReportsDiagnostic_WhenUsedInTaskReturningIndexer(string method);

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReportsNoDiagnostic_WhenUsedWithSyncMember(string method);
}