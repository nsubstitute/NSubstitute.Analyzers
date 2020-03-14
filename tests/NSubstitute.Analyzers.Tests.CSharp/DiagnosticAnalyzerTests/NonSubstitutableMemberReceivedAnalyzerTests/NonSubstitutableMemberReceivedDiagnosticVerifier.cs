using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedAnalyzerTests
{
    public abstract class NonSubstitutableMemberReceivedDiagnosticVerifier : CSharpDiagnosticVerifier, INonSubstitutableMemberReceivedDiagnosticVerifier
    {
        protected DiagnosticDescriptor NonVirtualReceivedSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualReceivedSetupSpecification;

        protected DiagnosticDescriptor InternalSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedAnalyzer();

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForNonSealedMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForDelegate(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForSealedMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForGenericInterfaceMethod(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForAbstractProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForInterfaceIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualProperty(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenCheckingReceivedCallsForVirtualIndexer(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostics_WhenCheckingReceivedCallsForNonVirtualIndexer(string method);

        [CombinatoryTheory]
        [InlineData(".Bar", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
        [InlineData(".FooBar()", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
        [InlineData("[0]", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

        [CombinatoryTheory]
        [InlineData(".Bar")]
        [InlineData(".FooBar()")]
        [InlineData("[0]")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

        [CombinatoryTheory]
        [InlineData(".Bar", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
        [InlineData(".FooBar()", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
        [InlineData("[0]", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
        public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

        [CombinatoryTheory]
        [InlineData(".Bar")]
        [InlineData(".FooBar()")]
        [InlineData("[0]")]
        public abstract Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);
    }
}