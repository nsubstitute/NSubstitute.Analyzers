using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.SyncOverAsyncThrowsAnalyzerTests;

public abstract class SyncOverAsyncThrowsDiagnosticVerifier : CSharpDiagnosticVerifier, ISyncOverAsyncThrowsDiagnosticVerifier
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