using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.DiagnosticAnalyzerTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : CSharpDiagnosticVerifier, IReEntrantReturnsSetupDiagnosticVerifier
    {
        protected DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;

        [CombinatoryTheory]
        [InlineData("substitute.Foo().Returns(1);")]
        [InlineData("OtherReturn(); substitute.Foo().Returns(1);")]
        [InlineData("substitute.Foo().Returns<int>(1);")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string method, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("OtherReturn(); substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string method, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("substitute.When(x => x.Foo()).Do(callInfo => { });")]
        [InlineData("OtherReturn(); substitute.When(x => x.Foo()).Do(callInfo => { });")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string method, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_ForNestedReEntrantCall(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_ForSpecificNestedReEntrantCall(string method);

        [CombinatoryTheory]
        [InlineData("var bar = Bar();")]
        [InlineData(@"var fooBar = Bar();
IBar bar;
bar = fooBar;")]
        public abstract Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string method, string localVariable);

        [CombinatoryTheory]
        [InlineData("MyMethod()", "substitute.Foo().Returns(1);")]
        [InlineData("MyProperty", "substitute.Foo().Returns(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().Returns(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().Returns(1);")]
        [InlineData("MyMethod()", "substitute.Foo().Returns<int>(1);")]
        [InlineData("MyProperty", "substitute.Foo().Returns<int>(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().Returns<int>(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().Returns<int>(1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.Returns(substitute.Foo(), 1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.Returns<int>(substitute.Foo(), 1);")]
        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string method, string rootCall, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().ReturnsForAnyArgs(1);")]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("x => ReturnThis()", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("x => { return ReturnThis(); }", "substitute.Foo().ReturnsForAnyArgs<int>(1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1);")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        [InlineData("x => ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        [InlineData("x => { return ReturnThis(); }", "SubstituteExtensions.ReturnsForAnyArgs<int>(substitute.Foo(), 1);")]
        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string method, string rootCall, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("x => 1", "x => 2")]
        public abstract Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string method, string firstReturn, string secondReturn);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles(string method);

        [CombinatoryTheory]
        [InlineData("Foo")]
        [InlineData("FooBar")]
        public abstract Task ReportsNoDiagnostic_WhenUsed_WithTypeofExpression(string method, string type);

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }
    }
}