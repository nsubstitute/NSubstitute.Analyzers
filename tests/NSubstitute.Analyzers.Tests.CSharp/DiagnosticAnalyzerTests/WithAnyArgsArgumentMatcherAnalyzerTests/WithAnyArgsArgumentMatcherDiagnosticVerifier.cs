using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public abstract class WithAnyArgsArgumentMatcherDiagnosticVerifier : ForAnyArgsArgumentMatcherDiagnosticVerifier, IWithAnyArgsArgumentMatcherDiagnosticVerifier
{
    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new WithAnyArgsArgumentMatcherAnalyzer();

    [CombinatoryTheory]
    [MemberData(nameof(MisusedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenUsingArgMatchersWithPropertyNotCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(ArgAnyTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithPropertyCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenAssigningArgMatchersToMemberNotPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(MisusedArgTestCasesDelegates))]
    public abstract Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethodForDelegate(string method, string arg);

    public static IEnumerable<object[]> MisusedArgTestCasesDelegates =>
        MisusedArgTestCases.Where(arguments => arguments.All(argument => argument.ToString().Contains("Invoke")));

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesDelegates =>
        CorrectlyUsedArgTestCases.Where(arguments => arguments.All(argument => argument.ToString().Contains("Invoke")));
}