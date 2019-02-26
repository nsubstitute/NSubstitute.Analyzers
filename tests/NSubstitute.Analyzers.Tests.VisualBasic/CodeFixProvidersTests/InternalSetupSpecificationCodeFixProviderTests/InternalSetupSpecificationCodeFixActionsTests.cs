using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.InternalSetupSpecificationCodeFixProviderTests
{
    public class InternalSetupSpecificationCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, IInternalSetupSpecificationCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreateCodeActions_InProperOrder()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

Namespace MyNamespace
    Public Class Foo
        Friend Overridable Function Bar() As Integer
            Return 1
        End Function
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Foo)()
            Dim x = substitute.Bar().Returns(1)
        End Sub
    End Class
End Namespace
";
            await VerifyCodeActions(source, "Add protected modifier", "Replace friend with public modifier", "Add InternalsVisibleTo attribute");
        }

        [Fact]
        public async Task DoesNotCreateCodeActions_WhenSymbol_DoesNotBelongToCompilation()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of Object)()
            Dim x = substitute.ToString().Returns(String.Empty)
        End Sub
    End Class
End Namespace";
            await VerifyCodeActions(source);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new NonSubstitutableMemberAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new InternalSetupSpecificationCodeFixProvider();
        }
    }
}