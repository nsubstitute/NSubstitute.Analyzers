using System.Threading.Tasks;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public abstract class UnusedReceivedDiagnosticVerifier : CSharpDiagnosticVerifier<UnusedReceivedAnalyzer>, IUnusedReceivedDiagnosticVerifier
    {
        [Fact]
        public abstract Task ReportDiagnostics_WhenUsedWithoutMemberCall();

        [Fact]
        public abstract Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess();

        [Fact]
        public abstract Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess();

        [Fact]
        public abstract Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess();

        [Fact]
        public abstract Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod();
    }
}