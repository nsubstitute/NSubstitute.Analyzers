using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Settings;
using NSubstitute.Analyzers.Shared.TinyJson;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberAnalyzerTests;

public abstract class NonSubstitutableMemberDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonSubstitutableMemberDiagnosticVerifier
{
    internal AnalyzersSettings? Settings { get; set; }

    protected DiagnosticDescriptor NonVirtualSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualSetupSpecification;

    protected DiagnosticDescriptor InternalSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberAnalyzer();

    protected override string? AnalyzerSettings => Settings != null ? Json.Encode(Settings) : null;

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method);

    [CombinatoryTheory]
    [InlineData("1", "Integer")]
    [InlineData(@"""c""C", "Char")]
    [InlineData("true", "Boolean")]
    [InlineData("false", "Boolean")]
    [InlineData(@"""1""", "String")]
    public abstract Task ReportsDiagnostics_WhenUsedWithLiteral(string method, string literal, string type);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithStaticMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithForNonSealedOverrideMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenDataFlowAnalysisIsRequired(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method);

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
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method);

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
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingNonVirtualGenericIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireType(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireGenericType(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingMembersFromEntireNamespace(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnosticsForSuppressedMember_WhenSuppressingExtensionMethod(string method);

    [CombinatoryTheory]
    [InlineData(".Bar", "Friend member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(".FooBar()", "Friend member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("(0)", "Friend member Item can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData(".Bar")]
    [InlineData(".FooBar()")]
    [InlineData("(0)")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    [CombinatoryTheory]
    [InlineData(".Bar", "Friend member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(".FooBar()", "Friend member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("(0)", "Friend member Item can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData(".Bar")]
    [InlineData(".FooBar()")]
    [InlineData("(0)")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);
}