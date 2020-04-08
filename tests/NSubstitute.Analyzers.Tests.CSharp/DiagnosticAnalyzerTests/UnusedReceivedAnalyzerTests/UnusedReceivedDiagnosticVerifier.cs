using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.UnusedReceivedAnalyzerTests
{
    public abstract class UnusedReceivedDiagnosticVerifier : CSharpDiagnosticVerifier, IUnusedReceivedDiagnosticVerifier
    {
        protected DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.UnusedReceived;

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new UnusedReceivedAnalyzer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportDiagnostics_WhenUsedWithoutMemberCall(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportNoDiagnostics_WhenUsedWithMethodMemberAccess(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportNoDiagnostics_WhenUsedWithPropertyMemberAccess(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportNoDiagnostics_WhenUsedWithIndexerMemberAccess(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportNoDiagnostics_WhenUsedWithInvokingDelegate(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsedWithUnfortunatelyNamedMethod(string method);
    }
}