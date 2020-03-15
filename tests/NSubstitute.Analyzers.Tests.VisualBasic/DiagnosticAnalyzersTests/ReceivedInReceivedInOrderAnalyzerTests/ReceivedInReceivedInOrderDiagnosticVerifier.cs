using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReceivedInReceivedInOrderAnalyzerTests
{
    public abstract class ReceivedInReceivedInOrderDiagnosticVerifier : VisualBasicDiagnosticVerifier, IReceivedInReceivedInOrderDiagnosticVerifier
    {
        protected DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReceivedUsedInReceivedInOrder;

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new ReceivedInReceivedInOrderAnalyzer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenUsingReceivedLikeMethodInReceivedInOrderBlock_ForIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostic_WhenUsingReceivedLikeMethodOutsideOfReceivedInOrderBlock(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);
    }
}