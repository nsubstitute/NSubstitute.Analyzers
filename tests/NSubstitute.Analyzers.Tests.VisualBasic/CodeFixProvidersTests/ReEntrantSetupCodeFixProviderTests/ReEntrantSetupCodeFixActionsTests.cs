using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ReEntrantSetupCodeFixProviderTests
{
    public class ReEntrantSetupCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, IReEntrantSetupCodeFixActionsVerifier
    {
        [Theory]
        [InlineData("Await CreateReEntrantSubstituteAsync(), Await CreateDefaultValue()")]
        [InlineData("CreateReEntrantSubstitute(), Await CreateDefaultValue()")]
        [InlineData("CreateReEntrantSubstitute(), new Integer() { 1, await CreateDefaultValue() }")]
        public async Task DoesNotCreateCodeFixAction_WhenAnyArgumentSyntaxIsAsync(string arguments)
        {
            var source = $@"Imports System.Threading.Tasks
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Async Function Test() As Task
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns({arguments})
        End Function

        Private Async Function CreateReEntrantSubstituteAsync() As Task(Of Integer)
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return Await Task.FromResult(1)
        End Function

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function

        Private Async Function CreateDefaultValue() As Task(Of Integer)
            If True Then
                Return Await Task.FromResult(1)
            End If
        End Function
    End Class
End Namespace
";
            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Theory]
        [InlineData("someArray")]
        [InlineData("Array.Empty(Of Integer)()")]
        public async Task DoesNotCreateCodeFixAction_WhenArrayParamsArgumentExpressionsCantBeRewritten(string paramsArgument)
        {
            var source = $@"Imports System
Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Interface IFoo
            ReadOnly Property Id As Integer
        End Interface

        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            Dim someArray = New Integer() {{1, 2, 3}}
            substitute.Id.Returns(CreateReEntrantSubstitute(), {paramsArgument})
        End Sub

        Private Function CreateReEntrantSubstitute() As Integer
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
            substitute.Id.Returns(1)
            Return 1
        End Function
    End Class
End Namespace
";
            await VerifyCodeActions(source, Array.Empty<string>());
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new ReEntrantSetupAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new ReEntrantSetupCodeFixProvider();
        }
    }
}