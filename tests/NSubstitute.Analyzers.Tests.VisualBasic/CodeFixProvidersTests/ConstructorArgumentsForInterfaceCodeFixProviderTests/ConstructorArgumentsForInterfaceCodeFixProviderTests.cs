using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ConstructorArgumentsForInterfaceCodeFixProviderTests
{
    public class ConstructorArgumentsForInterfaceCodeFixProviderTests : VisualBasicCodeFixVerifier, IConstructorArgumentsForInterfaceCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

        protected override CodeFixProvider CodeFixProvider { get; } = new ConstructorArgumentsForInterfaceCodeFixProvider();

        [Fact]
        public async Task RemovesInvocationArguments_WhenGenericFor_UsedWithParametersForInterface()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)(1, 2, 3)
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
            Dim substitute = NSubstitute.Substitute.[For](Of IFoo)()
        End Sub
    End Class
End Namespace
";

            await VerifyFix(source, newSource);
        }

        [Fact]
        public async Task RemovesInvocationArguments_WhenNonGenericFor_UsedWithParametersForInterface()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo)}, New Object() {1})
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo)}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyFix(source, newSource);
        }

        [Fact]
        public async Task RemovesInvocationArguments_WhenSubstituteFactoryCreate_UsedWithParametersForInterface()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.Create({GetType(IFoo)}, New Object() {1})
        End Sub
    End Class
End Namespace
";
            var newSource = @"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.Create({GetType(IFoo)}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyFix(source, newSource);
        }
    }
}