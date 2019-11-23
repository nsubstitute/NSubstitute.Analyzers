using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherDiagnosticVerifier : CSharpDiagnosticVerifier, IArgumentMatcherDiagnosticVerifier
    {
        internal AnalyzersSettings Settings { get; set; }

        protected DiagnosticDescriptor ArgumentMatcherUsedWithoutSpecifyingCall { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ArgumentMatcherUsedWithoutSpecifyingCall;

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

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }

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
                yield return new object[] { "[|Arg.Do<int>(_ => {})|]" };
                yield return new object[] { "[|Arg.Compat.Do<int>(_ => {})|]" };
                yield return new object[] { "[|Arg.Invoke()|]" };
                yield return new object[] { "[|Arg.Compat.Invoke()|]" };
                yield return new object[] { "[|Arg.InvokeDelegate<int>()|]" };
                yield return new object[] { "[|Arg.Compat.InvokeDelegate<int>()|]" };
            }
        }

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
                yield return new object[] { "Arg.Compat.Is(1)" };
                yield return new object[] { "(int)Arg.Compat.Is(1)" };
                yield return new object[] { "Arg.Compat.Is(1) as int?" };
                yield return new object[] { "Arg.Do<int>(_ => {})" };
                yield return new object[] { "Arg.Compat.Do<int>(_ => {})" };
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

        public static IEnumerable<object[]> CorrectlyUsedArgTestCasesWithoutCasts
        {
            get
            {
                yield return new object[] { "Arg.Any<int>()" };
                yield return new object[] { "Arg.Compat.Any<int>()" };
                yield return new object[] { "Arg.Is(1)" };
                yield return new object[] { "Arg.Compat.Is(1)" };
                yield return new object[] { "Arg.Do<int>(_ => {})" };
                yield return new object[] { "Arg.Compat.Do<int>(_ => {})" };
                yield return new object[] { "Arg.Invoke()" };
                yield return new object[] { "Arg.Compat.Invoke()" };
                yield return new object[] { "Arg.InvokeDelegate<int>()" };
                yield return new object[] { "Arg.Compat.InvokeDelegate<int>()" };
            }
        }

        protected override string GetSettings()
        {
            return Settings != null ? Json.Encode(Settings) : null;
        }
    }
}