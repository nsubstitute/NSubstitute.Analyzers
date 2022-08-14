using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public abstract class ForAnyArgsArgumentMatcherDiagnosticVerifier : VisualBasicDiagnosticVerifier, IForAnyArgsArgumentMatcherDiagnosticVerifier
{
    internal AnalyzersSettings Settings { get; set; }

    protected DiagnosticDescriptor WithAnyArgsArgumentMatcherUsage { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.WithAnyArgsArgumentMatcherUsage;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new WithAnyArgsArgumentMatcherAnalyzer();

    [CombinatoryTheory]
    [MemberData(nameof(MisusedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenUsingArgMatchersWithInvocationNotCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(ArgAnyTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithInvocationCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(MisusedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsDiagnostics_WhenUsingInvalidArgMatchersWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(CorrectlyUsedArgTestCasesWithoutDelegates))]
    public abstract Task ReportsNoDiagnostics_WhenUsingArgMatchersWithIndexerNotCombinedWithAnyArgsLikeMethod(string method, string arg);

    [CombinatoryTheory]
    [MemberData(nameof(ArgAnyTestCases))]
    public abstract Task ReportsNoDiagnostics_WhenUsingArgAnyMatcherWithIndexerCombinedWithAnyArgsLikeMethod(string method, string arg);

    public static IEnumerable<object[]> MisusedArgTestCases
    {
        get { return MisusedArgs.Select(argArray => argArray.Select<string, object>(arg => arg).ToArray()); }
    }

    public static IEnumerable<object[]> MisusedArgTestCasesWithoutDelegates =>
           MisusedArgTestCases.Where(arguments => arguments.All(argument => !argument.ToString().Contains("Invoke")));

    public static IEnumerable<object[]> ArgAnyTestCases =>
        CorrectlyUsedArgTestCases.Where(arguments => arguments.All(argument => argument.ToString().Contains("Any")));

    public static IEnumerable<object[]> MisusedArgTestCasesDelegates =>
        MisusedArgTestCases.Where(arguments => arguments.All(argument => argument.ToString().Contains("Invoke")));

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
            yield return new object[] { "Arg.Invoke(1)" };
            yield return new object[] { "Arg.Compat.Invoke(1)" };
            yield return new object[] { "Arg.InvokeDelegate(Of Action(Of Integer))()" };
            yield return new object[] { "Arg.Compat.InvokeDelegate(Of Action(Of Integer))()" };
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

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutDelegates => CorrectlyUsedArgTestCases.Where(arguments =>
            arguments.All(argument => !argument.ToString().Contains("Invoke")));

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesDelegates =>
        CorrectlyUsedArgTestCases.Where(arguments => arguments.All(argument => argument.ToString().Contains("Invoke")));

    public static IEnumerable<string[]> MisusedArgs
    {
        get
        {
            yield return new[] { "[|Arg.Is(1)|]" };
            yield return new[] { "TryCast([|Arg.Is(1)|], Object)" };
            yield return new[] { "CType([|Arg.Is(1)|], Integer)" };
            yield return new[] { "DirectCast([|Arg.Is(1)|], Integer)" };
            yield return new[] { "[|Arg.Compat.Is(1)|]" };
            yield return new[] { "TryCast([|Arg.Compat.Is(1)|], Object)" };
            yield return new[] { "CType([|Arg.Compat.Is(1)|], Integer)" };
            yield return new[] { "DirectCast([|Arg.Compat.Is(1)|], Integer)" };
            yield return new[] { "[|Arg.Invoke(1)|]" };
            yield return new[] { "[|Arg.Compat.Invoke(1)|]" };
            yield return new[] { "[|Arg.InvokeDelegate(Of Action(Of Integer))()|]" };
            yield return new[] { "[|Arg.Compat.InvokeDelegate(Of Action(Of Integer))()|]" };
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
}