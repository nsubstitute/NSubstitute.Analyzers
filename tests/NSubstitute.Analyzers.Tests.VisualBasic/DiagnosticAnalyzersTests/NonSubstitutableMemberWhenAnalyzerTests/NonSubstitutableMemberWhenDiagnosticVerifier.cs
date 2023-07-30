using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.NonSubstitutableMemberWhenAnalyzerTests;

public abstract class NonSubstitutableMemberWhenDiagnosticVerifier : VisualBasicDiagnosticVerifier, INonSubstitutableMemberWhenDiagnosticVerifier
{
    protected DiagnosticDescriptor NonVirtualWhenSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualWhenSetupSpecification;

    protected DiagnosticDescriptor InternalSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberWhenAnalyzer();

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMemberFromBaseClass(string method);

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
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularFunction(string method);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = [|sb.Bar|]
            End Sub",
        "Friend member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(@"Sub(sb As Foo) [|sb.FooBar(Arg.Any(Of Integer)())|]", "Friend member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = [|sb(Arg.Any(Of Integer)())|]
            End Sub",
        "Friend member Item can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
    [InlineData(@"Sub(sb As Foo) sb.FooBar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = [|sb.Bar|]
            End Sub",
        "Friend member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(@"Sub(sb As Foo) [|sb.FooBar(Arg.Any(Of Integer)())|]", "Friend member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = [|sb(Arg.Any(Of Integer)())|]
            End Sub",
        "Friend member Item can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
    [InlineData(@"Sub(sb As Foo) sb.FooBar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);
}