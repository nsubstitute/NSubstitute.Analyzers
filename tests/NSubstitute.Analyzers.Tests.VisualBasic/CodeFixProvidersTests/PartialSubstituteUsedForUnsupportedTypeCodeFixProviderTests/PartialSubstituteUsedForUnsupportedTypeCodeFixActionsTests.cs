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
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

        protected override CodeFixProvider CodeFixProvider { get; } = new PartialSubstituteUsedForUnsupportedTypeCodeFixProvider();

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
    }
}