using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.SyncOverAsyncThrowsCodeFixProviderTests;

public abstract class SyncOverAsyncThrowsCodeFixVerifier : CSharpCodeFixVerifier, ISyncOverAsyncThrowsCodeFixVerifier
{
    public static IEnumerable<object[]> ThrowsTestCases
    {
        get
        {
            yield return new object[] { "Throws", "Returns" };
            yield return new object[] { "ThrowsForAnyArgs", "ReturnsForAnyArgs" };
        }
    }

    public static IEnumerable<object[]> ThrowsAsyncTestCases
    {
        get
        {
            yield return new object[] { "Throws", "ThrowsAsync" };
            yield return new object[] { "ThrowsForAnyArgs", "ThrowsAsyncForAnyArgs" };
        }
    }

    protected override CodeFixProvider CodeFixProvider { get; } = new SyncOverAsyncThrowsCodeFixProvider();

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SyncOverAsyncThrowsAnalyzer();

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReplacesThrowsWithReturns_WhenUsedInMethod(string method, string updatedMethod);

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReplacesThrowsWithReturns_WhenUsedInProperty(string method, string updatedMethod);

    [Theory]
    [MemberData(nameof(ThrowsTestCases))]
    public abstract Task ReplacesThrowsWithReturns_WhenUsedInIndexer(string method, string updatedMethod);

    [Theory]
    [MemberData(nameof(ThrowsAsyncTestCases))]
    public abstract Task ReplacesThrowsWithThrowsAsync_WhenUsedInMethod(string method, string updatedMethod);

    [Theory]
    [MemberData(nameof(ThrowsAsyncTestCases))]
    public abstract Task ReplacesThrowsWithThrowsAsync_WhenUsedInProperty(string method, string updatedMethod);

    [Theory]
    [MemberData(nameof(ThrowsAsyncTestCases))]
    public abstract Task ReplacesThrowsWithThrowsAsync_WhenUsedInIndexer(string method, string updatedMethod);
}