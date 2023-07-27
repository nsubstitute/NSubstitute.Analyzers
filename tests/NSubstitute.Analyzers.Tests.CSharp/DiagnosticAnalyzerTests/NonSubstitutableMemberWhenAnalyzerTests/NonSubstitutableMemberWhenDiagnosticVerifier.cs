using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.NonSubstitutableMemberWhenAnalyzerTests;

public abstract class NonSubstitutableMemberWhenDiagnosticVerifier : CSharpDiagnosticVerifier, INonSubstitutableMemberWhenDiagnosticVerifier
{
    protected DiagnosticDescriptor NonVirtualSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.NonVirtualSetupSpecification;

    protected DiagnosticDescriptor InternalSetupSpecificationDescriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.InternalSetupSpecification;

    protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new NonSubstitutableMemberWhenAnalyzer();

    [CombinatoryTheory]
    [InlineData("sub => [|sub.Bar(Arg.Any<int>())|]")]
    [InlineData("delegate(Foo sub) { [|sub.Bar(Arg.Any<int>())|]; }")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => [|sub.Bar(Arg.Any<int>())|]")]
    [InlineData("delegate(Foo sub) { [|sub.Bar(Arg.Any<int>())|]; }")]
    [InlineData("sub => { [|sub.Bar(Arg.Any<int>())|]; }")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMemberFromBaseClass(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithNonSealedOverrideMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub()")]
    [InlineData("delegate(Func<int> sub) { sub(); }")]
    [InlineData("sub => { sub(); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithDelegate(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => [|sub.Bar(Arg.Any<int>())|]")]
    [InlineData("delegate(Foo sub) { [|sub.Bar(Arg.Any<int>())|]; }")]
    [InlineData("sub => { [|sub.Bar(Arg.Any<int>())|]; }")]
    public abstract Task ReportsDiagnostics_WhenUsedWithSealedOverrideMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
    [InlineData("sub => { int x; x = sub.Bar; }")]
    [InlineData("sub => { var x = sub.Bar; }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar<int>(Arg.Any<int>())")]
    [InlineData("delegate(Foo<int> sub) { sub.Bar<int>(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar<int>(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithGenericInterfaceMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => { int x; x = sub.Bar; }")]
    [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithAbstractProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("delegate(Foo sub) { var x = sub[Arg.Any<int>()]; }")]
    [InlineData("sub => { var x = sub[Arg.Any<int>()]; }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInterfaceIndexer(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub.Bar|]; }")]
    [InlineData("sub => { int x; x = [|sub.Bar|]; }")]
    [InlineData("delegate(Foo sub) { var x = [|sub.Bar|]; }")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => { int x; x = sub.Bar; }")]
    [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub[Arg.Any<int>()]|]; }")]
    [InlineData("delegate(Foo sub) { var x = [|sub[Arg.Any<int>()]|]; }")]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualIndexer(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InExpressionBodiedLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenUsedWithNonVirtualMember_InRegularExpressionBodiedFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InExpressionBodiedLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithVirtualMember_InRegularExpressionBodiedFunction(string method);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub.Bar|]; }", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => [|sub.FooBar(Arg.Any<int>())|]", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => { var x = [|sub[Arg.Any<int>()]|]; }", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => sub.FooBar(Arg.Any<int>())")]
    [InlineData("sub => { var x = sub[Arg.Any<int>()]; }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub.Bar|]; }", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => [|sub.FooBar(Arg.Any<int>())|]", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => { var x = [|sub[Arg.Any<int>()]|]; }", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenUsedWithInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => sub.FooBar(Arg.Any<int>())")]
    [InlineData("sub => { var x = sub[Arg.Any<int>()]; }")]
    public abstract Task ReportsNoDiagnostics_WhenUsedWithProtectedInternalVirtualMember(string method, string call);
}