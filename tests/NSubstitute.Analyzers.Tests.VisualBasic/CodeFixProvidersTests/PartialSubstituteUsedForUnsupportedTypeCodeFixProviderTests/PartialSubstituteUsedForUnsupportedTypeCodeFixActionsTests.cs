using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.PartialSubstituteUsedForUnsupportedTypeCodeFixProviderTests
{
    public class PartialSubstituteUsedForUnsupportedTypeCodeFixActionsTests : VisualBasicCodeFixActionsVerifier, IPartialSubstituteUsedForUnsupportedTypeCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForSubstituteForPartsOf()
        {
            var source = @"Imports NSubstitute

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
            await VerifyCodeActions(source, "Use Substitute.For");
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForSubstituteFactoryCreatePartial()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(IFoo)}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyCodeActions(source, "Use SubstituteFactory.Create");
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new PartialSubstituteUsedForUnsupportedTypeCodeFixProvider();
        }
    }
}