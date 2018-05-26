using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Test
{
    public abstract class UnusedReceivedAnalyzerTestBase : AnalyzerTest
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