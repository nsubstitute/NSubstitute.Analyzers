﻿using System.Threading.Tasks;
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
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => [|sub.Bar(Arg.Any<int>())|]")]
    [InlineData("delegate(Foo sub) { [|sub.Bar(Arg.Any<int>())|]; }")]
    [InlineData("sub => { [|sub.Bar(Arg.Any<int>())|]; }")]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMemberFromBaseClass(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForNonSealedOverrideMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub()")]
    [InlineData("delegate(Func<int> sub) { sub(); }")]
    [InlineData("sub => { sub(); }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForDelegate(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => [|sub.Bar(Arg.Any<int>())|]")]
    [InlineData("delegate(Foo sub) { [|sub.Bar(Arg.Any<int>())|]; }")]
    [InlineData("sub => { [|sub.Bar(Arg.Any<int>())|]; }")]
    public abstract Task ReportsDiagnostics_WhenSettingValueForSealedOverrideMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
    [InlineData("sub => { int x; x = sub.Bar; }")]
    [InlineData("sub => { var x = sub.Bar; }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar<int>(Arg.Any<int>())")]
    [InlineData("delegate(Foo<int> sub) { sub.Bar<int>(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar<int>(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForGenericInterfaceMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => { int x; x = sub.Bar; }")]
    [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForAbstractProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("delegate(Foo sub) { var x = sub[Arg.Any<int>()]; }")]
    [InlineData("sub => { var x = sub[Arg.Any<int>()]; }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForInterfaceIndexer(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => sub.Bar(Arg.Any<int>())")]
    [InlineData("delegate(Foo sub) { sub.Bar(Arg.Any<int>()); }")]
    [InlineData("sub => { sub.Bar(Arg.Any<int>()); }")]
    public abstract Task ReportsNoDiagnostics_WhenUsingUnfortunatelyNamedMethod(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub.Bar|]; }")]
    [InlineData("sub => { int x; x = [|sub.Bar|]; }")]
    [InlineData("delegate(Foo sub) { var x = [|sub.Bar|]; }")]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => { int x; x = sub.Bar; }")]
    [InlineData("delegate(Foo sub) { var x = sub.Bar; }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualProperty(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub[Arg.Any<int>()]|]; }")]
    [InlineData("delegate(Foo sub) { var x = [|sub[Arg.Any<int>()]|]; }")]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualIndexer(string method, string whenAction);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InExpressionBodiedLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsDiagnostics_WhenSettingValueForNonVirtualMember_InRegularExpressionBodiedFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InExpressionBodiedLocalFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularFunction(string method);

    [CombinatoryTheory]
    [InlineData]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForVirtualMember_InRegularExpressionBodiedFunction(string method);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub.Bar|]; }", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => [|sub.FooBar(Arg.Any<int>())|]", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => { var x = [|sub[Arg.Any<int>()]|]; }", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToNotApplied(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => sub.FooBar(Arg.Any<int>())")]
    [InlineData("sub => { var x = sub[Arg.Any<int>()]; }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToApplied(string method, string call);

    [CombinatoryTheory]
    [InlineData("sub => { var x = [|sub.Bar|]; }", "Internal member Bar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => [|sub.FooBar(Arg.Any<int>())|]", "Internal member FooBar can not be intercepted without InternalsVisibleToAttribute.")]
    [InlineData("sub => { var x = [|sub[Arg.Any<int>()]|]; }", "Internal member this[] can not be intercepted without InternalsVisibleToAttribute.")]
    public abstract Task ReportsDiagnostics_WhenSettingValueForInternalVirtualMember_AndInternalsVisibleToAppliedToWrongAssembly(string method, string call, string message);

    [CombinatoryTheory]
    [InlineData("sub => { var x = sub.Bar; }")]
    [InlineData("sub => sub.FooBar(Arg.Any<int>())")]
    [InlineData("sub => { var x = sub[Arg.Any<int>()]; }")]
    public abstract Task ReportsNoDiagnostics_WhenSettingValueForProtectedInternalVirtualMember(string method, string call);
}