using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public abstract class WithAnyArgsArgumentMatcherDiagnosticVerifier : ForAnyArgsArgumentMatcherDiagnosticVerifier, IWithAnyArgsArgumentMatcherDiagnosticVerifier
{
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
}