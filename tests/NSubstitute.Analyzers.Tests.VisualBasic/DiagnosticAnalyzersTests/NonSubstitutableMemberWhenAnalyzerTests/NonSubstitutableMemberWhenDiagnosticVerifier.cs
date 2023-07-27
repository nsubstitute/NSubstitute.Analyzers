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
    [InlineData("Sub(sb) [|sb.Bar(Arg.Any(Of Integer)())|]")]
    [InlineData(@"Function(ByVal [sub] As Foo) [|[sub].Bar(Arg.Any(Of Integer)())|]")]
    [InlineData(
        @"Sub(sb As Foo)
                [|sb.Bar(Arg.Any(Of Integer)())|]
            End Sub")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) [|sb.Bar(Arg.Any(Of Integer)())|]")]
    [InlineData(@"Function(ByVal [sub] As Foo) [|[sub].Bar(Arg.Any(Of Integer)())|]")]
    [InlineData(
        @"Sub(sb As Foo)
                [|sb.Bar(Arg.Any(Of Integer)())|]
            End Sub")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMemberFromBaseClass(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb.Bar(Arg.Any(Of Integer)())")]
    [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                sb.Bar(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb.Bar(Arg.Any(Of Integer)())")]
    [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                sb.Bar(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb()")]
    [InlineData(@"Function(ByVal [sub] As Func(Of Integer)) [sub]()")]
    [InlineData(
        @"Sub(sb As Func(Of Integer))
                sb()
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) [|sb.Bar(Arg.Any(Of Integer)())|]")]
    [InlineData(@"Function(ByVal [sub] As Foo) [|[sub].Bar(Arg.Any(Of Integer)())|]")]
    [InlineData(
        @"Sub(sb As Foo)
                [|sb.Bar(Arg.Any(Of Integer)())|]
            End Sub")]
    public abstract Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb.Bar(Arg.Any(Of Integer)())")]
    [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                sb.Bar(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb.Bar(Arg.Any(Of Integer)())")]
    [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                sb.Bar(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb.Bar(Of Integer)(Arg.Any(Of Integer)())")]
    [InlineData(@"Function(ByVal [sub] As Foo(Of Integer)) [sub].Bar(Of Integer)(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo(Of Integer))
                sb.Bar(Of Integer)(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("Sub(sb) sb.Bar(Arg.Any(Of Integer)())")]
    [InlineData(@"Function(ByVal [sub] As Foo) [sub].Bar(Arg.Any(Of Integer)())")]
    [InlineData(
        @"Sub(sb As Foo)
                sb.Bar(Arg.Any(Of Integer)())
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = [|sb.Bar|]
            End Sub")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x as Integer
                x = [|sb.Bar|]
            End Sub")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = sb.Bar
            End Sub")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x as Integer
                x = sb.Bar
            End Sub")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x = [|sb(Arg.Any(Of Integer)())|]
            End Sub")]
    [InlineData(
        @"Sub(sb As Foo)
                Dim x as Integer
                x = [|sb(Arg.Any(Of Integer)())|]
            End Sub")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method, string whenAction);

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