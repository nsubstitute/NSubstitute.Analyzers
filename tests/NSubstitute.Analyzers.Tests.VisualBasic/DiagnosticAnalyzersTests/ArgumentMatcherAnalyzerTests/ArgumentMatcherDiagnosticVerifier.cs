using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ArgumentMatcherAnalyzerTests
{
    public abstract class ArgumentMatcherDiagnosticVerifier : VisualBasicDiagnosticVerifier, IArgumentMatcherDiagnosticVerifier
    {
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

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ArgumentMatcherAnalyzer();
        }

        public static IEnumerable<object[]> MisusedArgTestCases
        {
            get
            {
                yield return new object[] { "[|Arg.Any(Of Integer)()|]" };
                yield return new object[] { "[|Arg.Compat.Any(Of Integer)()|]" };
                yield return new object[] { "[|Arg.Is(1)|]" };
                yield return new object[] { "[|Arg.Compat.Is(1)|]" };
            }
        }

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

                yield return new object[] { "Arg.Compat.Is(1)" };
                yield return new object[] { "TryCast(Arg.Compat.Is(1), Object)" };
                yield return new object[] { "CType(Arg.Compat.Is(1), Integer)" };
                yield return new object[] { "DirectCast(Arg.Compat.Is(1), Integer)" };
            }
        }
    }
}