using System.Threading.Tasks;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public abstract class UnusedReceivedDiagnosticVerifier : VisualBasicDiagnosticVerifier<UnusedReceivedAnalyzer>, IUnusedReceivedDiagnosticVerifier
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