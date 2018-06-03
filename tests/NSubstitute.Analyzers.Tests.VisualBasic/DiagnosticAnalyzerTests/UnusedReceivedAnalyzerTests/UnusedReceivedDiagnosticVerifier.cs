using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public abstract class UnusedReceivedDiagnosticVerifier : VisualBasicDiagnosticVerifier, IUnusedReceivedDiagnosticVerifier
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

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new UnusedReceivedAnalyzer();
        }
    }
}