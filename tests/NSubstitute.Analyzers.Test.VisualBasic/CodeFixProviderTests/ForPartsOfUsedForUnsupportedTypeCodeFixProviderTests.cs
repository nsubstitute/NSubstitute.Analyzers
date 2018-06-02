using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CodeFixProviders;
using NSubstitute.Analyzers.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Test.VisualBasic.CodeFixProviderTests
{
    public class ForPartsOfUsedForUnsupportedTypeCodeFixProviderTests : CodeFixProviderTest
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

            await VerifyVisualBasicFix(oldSource, newSource);
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

            await VerifyVisualBasicFix(oldSource, newSource);
        }

        protected override DiagnosticAnalyzer GetVisualBasicDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetVisualBasicCodeFixProvider()
        {
            return new ForPartsOfUsedForUnsupportedTypeCodeFixProvider();
        }
    }
}