using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberReceivedAnalyzerTests;

public abstract class NonSubstitutableMemberReceivedDiagnosticVerifier : CSharpDiagnosticVerifier, INonSubstitutableMemberReceivedDiagnosticVerifier
{
    protected DiagnosticDescriptor NonVirtualReceivedSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualReceivedSetupSpecification;

    protected DiagnosticDescriptor InternalSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberReceivedAnalyzer();

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualEvent(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractEvent(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualEvent(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceEvent(string method);

    [CombinatoryTheory]
    [InlineData(".Bar", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(".FooBar()", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("[0]", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData(".Bar")]
    [InlineData(".FooBar()")]
    [InlineData("[0]")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    [CombinatoryTheory]
    [InlineData(".Bar", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(".FooBar()", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("[0]", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData(".Bar")]
    [InlineData(".FooBar()")]
    [InlineData("[0]")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);
}