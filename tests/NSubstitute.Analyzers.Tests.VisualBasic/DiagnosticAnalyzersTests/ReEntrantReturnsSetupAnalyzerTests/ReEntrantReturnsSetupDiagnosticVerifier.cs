using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.Extensibility;
using NSubstitute.Analyzers.VisualBasic;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : VisualBasicDiagnosticVerifier, IReEntrantReturnsSetupDiagnosticVerifier
    {
        protected DiagnosticDescriptor Descriptor { get; } = DiagnosticDescriptors<DiagnosticDescriptorsProvider>.ReEntrantSubstituteCall;

        [CombinatoryTheory]
        [InlineData("substitute.Foo().Returns(1)")]
        [InlineData("substitute.Foo().Returns(1) \n\rOtherReturn()")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string method, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("OtherReturn()\r\n substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string method, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("substitute.[When](Function(x) x.Foo()).[Do](Function(callInfo) 1)")]
        [InlineData("OtherReturn() \r\n substitute.[When](Function(x) x.Foo()).[Do](Function(callInfo) 1)")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string method, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_ForNestedReEntrantCall(string method);

        [CombinatoryTheory]
        [InlineData]
        public abstract Task ReportsDiagnostic_ForSpecificNestedReEntrantCall(string method);

        [CombinatoryTheory]
        [InlineData("Dim barr = Bar()")]
        [InlineData(@"Dim fooBar = Bar()
Dim barr As IBar
barr = fooBar")]
        public abstract Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string method, string localVariable);

        [CombinatoryTheory]
        [InlineData("MyMethod()", "substitute.Foo().Returns(1)")]
        [InlineData("MyProperty", "substitute.Foo().Returns(1)")]
        [InlineData("Function(x) ReturnThis()", "substitute.Foo().Returns(1)")]
        [InlineData("Function(x) \r\n Return ReturnThis() \r\n End Function", "substitute.Foo().Returns(1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n Return ReturnThis() \r\n End Function", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n Return ReturnThis() \r\n End Function", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string method, string rootCall, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("Function(x) ReturnThis()", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("Function(x) \r\n Return ReturnThis() \r\n End Function", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n Return ReturnThis() \r\n End Function", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n Return ReturnThis() \r\n End Function", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string method, string rootCall, string reEntrantCall);

        [CombinatoryTheory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("Function(x) 1", "Function(x) 2")]
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