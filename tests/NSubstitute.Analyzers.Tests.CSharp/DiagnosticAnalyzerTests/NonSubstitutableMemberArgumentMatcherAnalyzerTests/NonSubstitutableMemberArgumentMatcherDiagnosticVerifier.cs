using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberArgumentMatcherAnalyzerTests;

public abstract class NonSubstitutableMemberArgumentMatcherDiagnosticVerifier : CSharpDiagnosticVerifier, INonSubstitutableMemberArgumentMatcherDiagnosticVerifier
{
    internal AnalyzersSettings? Settings { get; set; }

    protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonSubstitutableMemberArgumentMatcherUsage;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberArgumentMatcherAnalyzer();

    protected override string? AnalyzerSettings => Settings != null ? Json.Encode(Settings) : null;

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInNonVirtualMethod(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInStaticMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInVirtualMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInNonSealedOverrideMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInDelegate(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInSealedOverrideMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInAbstractMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInInterfaceMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInGenericInterfaceMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInInterfaceIndexer(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInVirtualIndexer(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInNonVirtualIndexer(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithPotentiallyValidAssignment(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedAsStandaloneExpression(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInConstructor(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToNotApplied(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToApplied(string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCases))]
    public abstract Task ReportsDiagnostics_WhenUsedInInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsedInProtectedInternalVirtualMember(string arg);

    [Theory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutCasts))]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string arg);

    [Fact]
    public abstract Task ReportsNoDiagnostics_WhenSubscribingToEvent();

    [Theory]
    [MemberData(nameof(AssignableArgMatchers))]
    public abstract Task ReportsNoDiagnostics_WhenAssigningAllowedArgMatchersToSubstitutableMember(string arg);

    [CombinatoryTheory]
    [MemberData(nameof(MisusedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenAssigningArgMatchersToSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg);

    [Theory]
    [MemberData(nameof(MisusedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember(string arg);

    [Theory]
    [MemberData(nameof(NotAssignableArgMatchers))]
    public abstract Task ReportsDiagnostics_WhenDirectlyAssigningNotAllowedArgMatchersToMember(string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenAssigningInvalidArgMatchersToMemberPrecededByWithAnyArgsLikeMethod(string receivedMethod, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenUsedDirectlyWithReturnStatement(string arg);

    public static IEnumerable<object[]> MisusedArgTestCases
    {
        get
        {
            yield return new object[] { "[|Arg.Any<int>()|]" };
            yield return new object[] { "(int)[|Arg.Any<int>()|]" };
            yield return new object[] { "[|Arg.Any<int>()|] as int?" };
            yield return new object[] { "[|Arg.Compat.Any<int>()|]" };
            yield return new object[] { "(int)[|Arg.Compat.Any<int>()|]" };
            yield return new object[] { "[|Arg.Compat.Any<int>()|] as int?" };
            yield return new object[] { "[|Arg.Is(1)|]" };
            yield return new object[] { "(int)[|Arg.Is(1)|]" };
            yield return new object[] { "[|Arg.Is(1)|] as int?" };
            yield return new object[] { "[|Arg.Compat.Is(1)|]" };
            yield return new object[] { "(int)[|Arg.Compat.Is(1)|]" };
            yield return new object[] { "[|Arg.Compat.Is(1)|] as int?" };
            yield return new object[] { "[|Arg.Do<int>(__ => {})|]" };
            yield return new object[] { "[|Arg.Compat.Do<int>(__ => {})|]" };
            yield return new object[] { "[|Arg.Invoke()|]" };
            yield return new object[] { "[|Arg.Compat.Invoke()|]" };
            yield return new object[] { "[|Arg.InvokeDelegate<int>()|]" };
            yield return new object[] { "[|Arg.Compat.InvokeDelegate<int>()|]" };
        }
    }

    public static IEnumerable<object[]> MisusedArgTestCasesWithoutDelegates =>
        MisusedArgTestCases.Where(arguments => arguments.All(argument => !argument.ToString()?.Contains("Invoke") ?? false));

    public static IEnumerable<object[]> CorrectlyUsedArgTestCases
    {
        get
        {
            yield return new object[] { "Arg.Any<int>()" };
            yield return new object[] { "(int)Arg.Any<int>()" };
            yield return new object[] { "Arg.Any<int>() as int?" };
            yield return new object[] { "Arg.Compat.Any<int>()" };
            yield return new object[] { "(int)Arg.Compat.Any<int>()" };
            yield return new object[] { "Arg.Compat.Any<int>() as int?" };
            yield return new object[] { "Arg.Is(1)" };
            yield return new object[] { "(int)Arg.Is(1)" };
            yield return new object[] { "Arg.Is(1) as int?" };
            yield return new object[] { "Arg.Is((int __) => __ > 0)" };
            yield return new object[] { "true ? Arg.Is((int __) => __ > 0) : 0" };
            yield return new object[] { "Arg.Compat.Is(1)" };
            yield return new object[] { "(int)Arg.Compat.Is(1)" };
            yield return new object[] { "Arg.Compat.Is(1) as int?" };
            yield return new object[] { "Arg.Compat.Is((int __) => __ > 0) " };
            yield return new object[] { "true ? Arg.Compat.Is((int __) => __ > 0) : 0" };
            yield return new object[] { "Arg.Do<int>(__ => {})" };
            yield return new object[] { "Arg.Compat.Do<int>(__ => {})" };
            yield return new object[] { "Arg.Invoke()" };
            yield return new object[] { "Arg.Compat.Invoke()" };
            yield return new object[] { "Arg.InvokeDelegate<int>()" };
            yield return new object[] { "(int)Arg.InvokeDelegate<int>()" };
            yield return new object[] { "Arg.InvokeDelegate<int>() as int?" };
            yield return new object[] { "Arg.Compat.InvokeDelegate<int>()" };
            yield return new object[] { "(int)Arg.Compat.InvokeDelegate<int>()" };
            yield return new object[] { "Arg.Compat.InvokeDelegate<int>() as int?" };
        }
    }

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutDelegates =>
        CorrectlyUsedArgTestCases.Where(arguments => arguments.All(argument => !argument.ToString()?.Contains("Invoke") ?? false));

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutCasts
    {
        get
        {
            yield return new object[] { "Arg.Any<int>()" };
            yield return new object[] { "Arg.Compat.Any<int>()" };
            yield return new object[] { "Arg.Is(1)" };
            yield return new object[] { "Arg.Compat.Is(1)" };
            yield return new object[] { "Arg.Do<int>(__ => {})" };
            yield return new object[] { "Arg.Compat.Do<int>(__ => {})" };
            yield return new object[] { "Arg.Invoke()" };
            yield return new object[] { "Arg.Compat.Invoke()" };
            yield return new object[] { "Arg.InvokeDelegate<int>()" };
            yield return new object[] { "Arg.Compat.InvokeDelegate<int>()" };
        }
    }

    public static IEnumerable<object[]> AssignableArgMatchers
    {
        get
        {
            yield return new object[] { "Arg.Do<int>(__ => {})" };
            yield return new object[] { "Arg.Compat.Do<int>(__ => {})" };
        }
    }

    public static IEnumerable<object[]> NotAssignableArgMatchers
    {
        get
        {
            yield return new object[] { "[|Arg.Any<int>()|]" };
            yield return new object[] { "[|Arg.Compat.Any<int>()|]" };
            yield return new object[] { "[|Arg.Is(1)|]" };
            yield return new object[] { "[|Arg.Compat.Is(1)|]" };
        }
    }
}