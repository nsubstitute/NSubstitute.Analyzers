using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonVirtualSetupReceivedAnalyzerTests
{
    public abstract class NonVirtualSetupReceivedDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonVirtualSetupReceivedDiagnosticVerifier
    {
        [Fact]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer();

        [Fact]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer();

        [Fact]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonVirtualSetupReceivedAnalyzer();
        }
    }
}