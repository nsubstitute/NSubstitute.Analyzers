using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.ReEntrantReturnsSetupAnalyzerTests
{
    public abstract class ReEntrantReturnsSetupDiagnosticVerifier : VisualBasicDiagnosticVerifier, IReEntrantReturnsSetupDiagnosticVerifier
    {
        [Theory]
        [InlineData("substitute.Foo().Returns(1)")]
        [InlineData("substitute.Foo().Returns(1) \n\rOtherReturn()")]
        [InlineData("SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsViaMethodCall(string reEntrantCall);

        [Theory]
        [InlineData("substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("OtherReturn()\r\n substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturnsForAnyArgsViaMethodCall(string reEntrantCall);

        [Theory]
        [InlineData("substitute.[When](Function(x) x.Foo()).[Do](Function(callInfo) 1)")]
        [InlineData("OtherReturn() \r\n substitute.[When](Function(x) x.Foo()).[Do](Function(callInfo) 1)")]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantWhenDo(string reEntrantCall);

        [Fact]
        public abstract Task ReportsDiagnostic_ForNestedReEntrantCall();

        [Fact]
        public abstract Task ReportsDiagnostic_ForSpecificNestedReEntrantCall();

        [Theory]
        [InlineData("Dim barr = Bar()")]
        [InlineData(@"Dim fooBar = Bar()
Dim barr As IBar
barr = fooBar")]
        public abstract Task ReportsNoDiagnostic_WhenReturnsValueIsCreated_BeforeSetup(string localVariable);

        [Theory]
        [InlineData("MyMethod()", "substitute.Foo().Returns(1)")]
        [InlineData("MyProperty", "substitute.Foo().Returns(1)")]
        [InlineData("Function(x) ReturnThis()", "substitute.Foo().Returns(1)")]
        [InlineData("Function(x) \r\n ReturnThis() \r\n End Function", "substitute.Foo().Returns(1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n ReturnThis() \r\n End Function", "SubstituteExtensions.Returns(substitute.Foo(), 1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n ReturnThis() \r\n End Function", "SubstituteExtensions.Returns(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsCallExists(string rootCall, string reEntrantCall);

        [Theory]
        [InlineData("MyMethod()", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("MyProperty", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("Function(x) ReturnThis()", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("Function(x) \r\n ReturnThis() \r\n End Function", "substitute.Foo().ReturnsForAnyArgs(1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n ReturnThis() \r\n End Function", "SubstituteExtensions.ReturnsForAnyArgs(substitute.Foo(), 1)")]
        [InlineData("MyMethod()", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("MyProperty", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) ReturnThis()", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        [InlineData("Function(x) \r\n ReturnThis() \r\n End Function", "SubstituteExtensions.ReturnsForAnyArgs(Of Integer)(substitute.Foo(), 1)")]
        public abstract Task ReportsNoDiagnostic_WhenRootCallCalledWithDelegate_AndReEntrantReturnsForAnyArgsCallExists(string rootCall, string reEntrantCall);

        [Theory]
        [InlineData("ReturnThis()", "OtherReturn()")]
        [InlineData("ReturnThis", "OtherReturn")]
        [InlineData("1", "2")]
        [InlineData("Function(x) 1", "Function(x) 2")]
        public abstract Task ReportsNoDiagnostic_WhenReEntrantSubstituteNotUsed(string firstReturn, string secondReturn);

        [Fact]
        public abstract Task ReportsDiagnostic_WhenUsingReEntrantReturns_AcrossMultipleFiles();

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }
    }
}