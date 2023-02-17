using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.WithAnyArgsArgumentMatcherAnalyzerTests;

public abstract class ForAnyArgsArgumentMatcherDiagnosticVerifier : CSharpDiagnosticVerifier, IForAnyArgsArgumentMatcherDiagnosticVerifier
{
    internal AnalyzersSettings? Settings { get; set; }

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
        get
        {
            yield return new object[] { "[|Arg.Is(1)|]" };
            yield return new object[] { "(int)[|Arg.Is(1)|]" };
            yield return new object[] { "[|Arg.Is(1)|] as int?" };
            yield return new object[] { "[|Arg.Compat.Is(1)|]" };
            yield return new object[] { "(int)[|Arg.Compat.Is(1)|]" };
            yield return new object[] { "[|Arg.Compat.Is(1)|] as int?" };
            yield return new object[] { "[|Arg.Do<int>(__ => {})|]" };
            yield return new object[] { "[|Arg.Compat.Do<int>(__ => {})|]" };
            yield return new object[] { "[|Arg.Invoke(1)|]" };
            yield return new object[] { "[|Arg.Compat.Invoke(1)|]" };
            yield return new object[] { "[|Arg.InvokeDelegate<Action<int>>()|]" };
            yield return new object[] { "[|Arg.Compat.InvokeDelegate<Action<int>>()|]" };
        }
    }

    public static IEnumerable<object[]> MisusedArgTestCasesWithoutDelegates =>
        MisusedArgTestCases.Where(arguments => arguments.All(argument => !argument.ToString()?.Contains("Invoke") ?? false));

    public static IEnumerable<object[]> ArgAnyTestCases =>
        CorrectlyUsedArgTestCases.Where(arguments => arguments.All(argument => argument.ToString()?.Contains("Any") ?? false));

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
            yield return new object[] { "Arg.Invoke(1)" };
            yield return new object[] { "Arg.Compat.Invoke(1)" };
            yield return new object[] { "Arg.InvokeDelegate<Action<int>>()" };
            yield return new object[] { "Arg.Compat.InvokeDelegate<Action<int>>()" };
        }
    }

    public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutDelegates =>
        CorrectlyUsedArgTestCases.Where(arguments => arguments.All(argument => !argument.ToString()?.Contains("Invoke") ?? false));
}