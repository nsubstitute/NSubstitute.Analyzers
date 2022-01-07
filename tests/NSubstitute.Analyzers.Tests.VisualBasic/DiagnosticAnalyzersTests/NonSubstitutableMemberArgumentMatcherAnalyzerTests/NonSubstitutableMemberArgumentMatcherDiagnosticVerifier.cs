using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberArgumentMatcherAnalyzerTests;

public abstract class NonSubstitutableMemberArgumentMatcherDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonSubstitutableMemberArgumentMatcherDiagnosticVerifier
{
    internal AnalyzersSettings Settings { get; set; }

    protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonSubstitutableMemberArgumentMatcherUsage;

    protected override string AnalyzerSettings => Settings != null ? Json.Encode(Settings) : null;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberArgumentMatcherAnalyzer();

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
    [MemberData(nameof(MisusedArgTestCasesWithoutCast))]
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
    public abstract Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember(string arg);

    [Theory]
    [MemberData(nameof(NotAssignableArgMatchers))]
    public abstract Task ReportsDiagnostics_WhenDirectlyAssigningNotAllowedArgMatchersToMember(string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenAssigningArgMatchersToSubstitutableMemberPrecededByReceivedLikeMethod(string receivedMethod, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(MisusedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenAssigningArgMatchersToNonSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenAssigningArgMatchersToSubstitutableMember_InWhenLikeMethod(string whenMethod, string arg);

    // VisualBasic specific case
    [Fact]
    public abstract Task ReportsNoDiagnostic_WhenOverloadCannotBeInferred();

    public static IEnumerable<object[]> MisusedArgTestCases
    {
        get { return MisusedArgs.Select(argArray => argArray.Select<string, object>(arg => arg).ToArray()); }
    }

    public static IEnumerable<object[]> MisusedArgTestCasesWithoutCast
    {
        get
        {
            var ignoredExpressions = new[] { "TryCast", "CType", "DirectCast" };

            return MisusedArgs.Where(args => args.Any(arg => !ignoredExpressions.Any(arg.Contains)));
        }
    }

    public static IEnumerable<object[]> MisusedArgTestCasesWithoutDelegates =>
        MisusedArgTestCases.Where(arguments => arguments.All(argument => !argument.ToString().Contains("Invoke")));

    public static IEnumerable<object[]> CorrectlyUsedArgTestCases
    {
        get
        {
            yield return new object[] { "Arg.Any(Of Integer)()" };
            yield return new object[] { "TryCast(Arg.Any(Of Integer)(), Object)" };
            yield return new object[] { "CType(Arg.Any(Of Integer)(), Integer)" };
            yield return new object[] { "DirectCast(Arg.Any(Of Integer)(), Integer)" };
            yield return new object[] { "Arg.Compat.Any(Of Integer)()" };
            yield return new object[] { "TryCast(Arg.Compat.Any(Of Integer)(), Object)" };
            yield return new object[] { "CType(Arg.Compat.Any(Of Integer)(), Integer)" };
            yield return new object[] { "DirectCast(Arg.Compat.Any(Of Integer)(), Integer)" };
            yield return new object[] { "Arg.Is(1)" };
            yield return new object[] { "TryCast(Arg.Is(1), Object)" };
            yield return new object[] { "CType(Arg.Is(1), Integer)" };
            yield return new object[] { "DirectCast(Arg.Is(1), Integer)" };
            yield return new object[] { "Arg.Is(Function(ByVal __ As Integer) __ > 0)" };
            yield return new object[] { "If(True, Arg.Is(Function(ByVal __ As Integer) __ > 0), 0)" };
            yield return new object[] { "Arg.Compat.Is(1)" };
            yield return new object[] { "TryCast(Arg.Compat.Is(1), Object)" };
            yield return new object[] { "CType(Arg.Compat.Is(1), Integer)" };
            yield return new object[] { "DirectCast(Arg.Compat.Is(1), Integer)" };
            yield return new object[] { "Arg.Compat.Is(Function(ByVal __ As Integer) __ > 0)" };
            yield return new object[] { "If(True, Arg.Compat.Is(Function(ByVal __ As Integer) __ > 0), 0)" };
            yield return new object[] { "Arg.Invoke()" };
            yield return new object[] { "Arg.Compat.Invoke()" };
            yield return new object[] { "Arg.InvokeDelegate(Of Integer)()" };
            yield return new object[] { "Arg.Compat.InvokeDelegate(Of Integer)()" };
            yield return new object[]
            {
                @"Arg.Do(Of Integer)(Function(doValue)
End Function)"
            };
            yield return new object[]
            {
                @"Arg.Compat.Do(Of Integer)(Function(doValue)
End Function)"
            };
        }
    }

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutDelegates =>
        CorrectlyUsedArgTestCases.Where(arguments =>
            arguments.All(argument => !argument.ToString().Contains("Invoke")));

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutCasts
    {
        get
        {
            yield return new object[] { "Arg.Any(Of Integer)()" };
            yield return new object[] { "Arg.Compat.Any(Of Integer)()" };
            yield return new object[] { "Arg.Is(1)" };
            yield return new object[] { "Arg.Compat.Is(1)" };
            yield return new object[] { "Arg.Invoke()" };
            yield return new object[] { "Arg.Compat.Invoke()" };
            yield return new object[] { "Arg.InvokeDelegate(Of Integer)()" };
            yield return new object[] { "Arg.Compat.InvokeDelegate(Of Integer)()" };
            yield return new object[]
            {
                @"Arg.Do(Of Integer)(Function(doValue)
End Function)"
            };
            yield return new object[]
            {
                @"Arg.Compat.Do(Of Integer)(Function(doValue)
End Function)"
            };
        }
    }

    public static IEnumerable<string[]> MisusedArgs
    {
        get
        {
            yield return new[] { "[|Arg.Any(Of Integer)()|]" };
            yield return new[] { "TryCast([|Arg.Any(Of Integer)()|], Object)" };
            yield return new[] { "CType([|Arg.Any(Of Integer)()|], Integer)" };
            yield return new[] { "DirectCast([|Arg.Any(Of Integer)()|], Integer)" };
            yield return new[] { "[|Arg.Compat.Any(Of Integer)()|]" };
            yield return new[] { "TryCast([|Arg.Compat.Any(Of Integer)()|], Object)" };
            yield return new[] { "CType([|Arg.Compat.Any(Of Integer)()|], Integer)" };
            yield return new[] { "DirectCast([|Arg.Compat.Any(Of Integer)()|], Integer)" };
            yield return new[] { "[|Arg.Is(1)|]" };
            yield return new[] { "TryCast([|Arg.Is(1)|], Object)" };
            yield return new[] { "CType([|Arg.Is(1)|], Integer)" };
            yield return new[] { "DirectCast([|Arg.Is(1)|], Integer)" };
            yield return new[] { "[|Arg.Compat.Is(1)|]" };
            yield return new[] { "TryCast([|Arg.Compat.Is(1)|], Object)" };
            yield return new[] { "CType([|Arg.Compat.Is(1)|], Integer)" };
            yield return new[] { "DirectCast([|Arg.Compat.Is(1)|], Integer)" };
            yield return new[] { "[|Arg.Invoke()|]" };
            yield return new[] { "[|Arg.Compat.Invoke()|]" };
            yield return new[] { "[|Arg.InvokeDelegate(Of Integer)()|]" };
            yield return new[] { "[|Arg.Compat.InvokeDelegate(Of Integer)()|]" };
            yield return new[]
            {
                @"[|Arg.Do(Of Integer)(Function(doValue)
End Function)|]"
            };
            yield return new[]
            {
                @"[|Arg.Compat.Do(Of Integer)(Function(doValue)
End Function)|]"
            };
        }
    }

    public static IEnumerable<object[]> AssignableArgMatchers
    {
        get
        {
            yield return new object[]
            {
                @"Arg.Do(Of Integer)(Function(doValue)
End Function)"
            };
            yield return new object[]
            {
                @"Arg.Compat.Do(Of Integer)(Function(doValue)
End Function)"
            };
        }
    }

    public static IEnumerable<object[]> NotAssignableArgMatchers
    {
        get
        {
            yield return new object[] { "[|Arg.Any(Of Integer)()|]" };
            yield return new object[] { "[|Arg.Compat.Any(Of Integer)()|]" };
            yield return new object[] { "[|Arg.Is(1)|]" };
            yield return new object[] { "[|Arg.Compat.Is(1)|]" };
        }
    }
}