using System.Threading.Tasks;

namespace NSubstitute.Analyzers.Test.CSharp.AnalyzerTests.UnusedReceivedAnalyzerTests
{
    public class ReceivedAsOrdinaryMethodWithGenericTypeSpecifiedTests : UnusedReceivedAnalyzerTests
    {
        public override Task ReportDiagnostics_WhenUsedWithoutMemberCall()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess()
        {
            throw new System.NotImplementedException();
        }

        public override Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess()
        {
            throw new System.NotImplementedException();
        }
    }
}