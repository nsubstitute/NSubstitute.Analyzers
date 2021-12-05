using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.AsyncThrowsAnalyzerTests
{
    public abstract class AsyncThrowsDiagnosticVerifier : VisualBasicDiagnosticVerifier, IAsyncThrowsDiagnosticVerifier
    {
        protected DiagnosticDescriptor AsyncThrowsDescriptor => DiagnosticDescriptors<DiagnosticDescriptorsProvider>.AsyncThrows;

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new AsyncThrowsAnalyzer();

        [Theory]
        [InlineData("Throws")]
        [InlineData("ThrowsForAnyArgs")]
        public abstract Task ReportsDiagnostic_WhenUsedWithNonGenericAsyncMethod(string method);

        [Theory]
        [InlineData("Throws")]
        [InlineData("ThrowsForAnyArgs")]
        public abstract Task ReportsDiagnostic_WhenUsedWithGenericAsyncMethod(string method);

        [Theory]
        [InlineData("Throws")]
        [InlineData("ThrowsForAnyArgs")]
        public abstract Task ReportsNoDiagnostic_WhenUsedWithSyncMethod(string method);
    }
}