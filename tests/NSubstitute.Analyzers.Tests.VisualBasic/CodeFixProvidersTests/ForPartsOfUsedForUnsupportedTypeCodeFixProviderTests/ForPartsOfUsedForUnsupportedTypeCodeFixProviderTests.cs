using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ForPartsOfUsedForUnsupportedTypeCodeFixProviderTests
{
    public class ForPartsOfUsedForUnsupportedTypeCodeFixProviderTests : VisualBasicCodeFixVerifier, IForPartsOfUsedForUnsupportedTypeCodeFixVerifier
    {
        [Fact]
        public async Task ReplacesForPartsOf_WithFor_WhenUsedWithInterface()
        {
            var oldSource = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of IFoo)()
        End Sub
    End Class
End Namespace
";
            var newSource = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of IFoo)()
        End Sub
    End Class
End Namespace
";

            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public async Task ReplacesForPartsOf_WithFor_WhenUsedWithDelegate()
        {
            var oldSource = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of System.Func(Of Integer))()
        End Sub
    End Class
End Namespace
";
            var newSource = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.For(Of System.Func(Of Integer))()
        End Sub
    End Class
End Namespace
";

            await VerifyFix(oldSource, newSource);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new ForPartsOfUsedForUnsupportedTypeCodeFixProvider();
        }
    }
}