using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.CodeFixProviders;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests.ConstructorArgumentsForInterfaceCodeFixProviderTests;

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
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)(New Integer() { 1, 2, 3 })
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
            Dim otherSubstitute = NSubstitute.Substitute.[For](Of IFoo)()
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
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo)}, constructorArguments:= New Object() {1})
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:= New Object() {1}, typesToProxy:= {GetType(IFoo)})
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
            Dim otherSubstitute = NSubstitute.Substitute.[For](typesToProxy:= {GetType(IFoo)}, constructorArguments:=Nothing)
            Dim yetAnotherSubstitute = NSubstitute.Substitute.[For](constructorArguments:=Nothing, typesToProxy:= {GetType(IFoo)})
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
            Dim otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy:= {GetType(IFoo)}, constructorArguments:= New Object() {1})
            Dim yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments:= New Object() {1}, typesToProxy:= {GetType(IFoo)})
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
            Dim otherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(typesToProxy:= {GetType(IFoo)}, constructorArguments:=Nothing)
            Dim yetAnotherSubstitute = SubstitutionContext.Current.SubstituteFactory.Create(constructorArguments:=Nothing, typesToProxy:= {GetType(IFoo)})
        End Sub
    End Class
End Namespace
";
        await VerifyFix(source, newSource);
    }
}