using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;
using NSubstitute.Analyzers.VisualBasic.CodeRefactoringProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeRefactoringProvidersTests.IntroduceSubstituteRefactoringProviderTests;

public class IntroduceSubstituteCodeRefactoringProviderTests : VisualBasicCodeRefactoringProviderVerifier, IIntroduceSubstituteCodeRefactoringProviderVerifier
{
    public static IEnumerable<object[]> AllArgumentsMissingTestCases
    {
        get
        {
            yield return new object[] { "New Foo([||])", "New Foo(service, otherService)" };
            yield return new object[] { "New Foo(, [||])", "New Foo(service, otherService)" };
            yield return new object[] { "New Foo([||],)", "New Foo(service, otherService)" };
            yield return new object[] { "New Foo([||]", "New Foo(service, otherService" };
            yield return new object[] { "New Foo(, [||]", "New Foo(service, otherService" };
            yield return new object[] { "New Foo([||],", "New Foo(service, otherService" };
        }
    }

    public static IEnumerable<object[]> SomeArgumentMissingTestCases
    {
        get
        {
            yield return new object[]
            {
                "New Foo([||],someOtherService, someAnotherService, )",
                "New Foo(service, someOtherService, someAnotherService, yetAnotherService)"
            };
            yield return new object[]
            {
                "New Foo([||],someOtherService, someAnotherService, )",
                "New Foo(service, someOtherService, someAnotherService, yetAnotherService)"
            };
            yield return new object[]
            {
                "New Foo(,someOtherService, someAnotherService, [||])",
                "New Foo(service, someOtherService, someAnotherService, yetAnotherService)"
            };
            yield return new object[]
            {
                "New Foo([||],someOtherService, someAnotherService,",
                "New Foo(service, someOtherService, someAnotherService, yetAnotherService"
            };
            yield return new object[]
            {
                "New Foo(,someOtherService, someAnotherService, [||]",
                "New Foo(service, someOtherService, someAnotherService, yetAnotherService"
            };
        }
    }

    public static IEnumerable<object[]> SpecificArgumentMissingTestCases
    {
        get
        {
            yield return new object[]
            {
                "New Foo(service, [||],, someYetAnotherService)",
                "New Foo(service, otherService,, someYetAnotherService)"
            };

            yield return new object[]
            {
                "New Foo(service, [||],, someYetAnotherService)",
                "New Foo(service, otherService,, someYetAnotherService)"
            };
        }
    }

    protected override CodeRefactoringProvider CodeRefactoringProvider { get; } = new IntroduceSubstituteCodeRefactoringProvider();

    [Theory]
    [MemberData(nameof(AllArgumentsMissingTestCases))]
    public async Task GeneratesConstructorArgumentListWithLocalVariables_WhenAllArgumentsMissing(string creationExpression, string expectedCreationExpression)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim target = {creationExpression}
        End Sub
    End Class
End Namespace
";
        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = Substitute.For(Of IService)()
            Dim otherService = Substitute.For(Of IOtherService)()
            Dim target = {expectedCreationExpression}
        End Sub
    End Class
End Namespace
";
        await VerifyRefactoring(source, newSource, refactoringIndex: 2);
    }

    [Fact]
    public async Task GeneratesConstructorArgumentListForAllArguments_WithFullyQualifiedName_WhenNamespaceNotImported()
    {
        var source = @"
Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim target = New Foo([||])
        End Sub
    End Class
End Namespace";

        var newSource = @"
Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = NSubstitute.Substitute.For(Of IService)()
            Dim otherService = NSubstitute.Substitute.For(Of IOtherService)()
            Dim target = New Foo(service, otherService)
        End Sub
    End Class
End Namespace";

        await VerifyRefactoring(source, newSource, refactoringIndex: 2);
    }

    [Fact]
    public async Task GeneratesConstructorArgumentListForSpecificSubstitute_WithFullyQualifiedName_WhenNamespaceNotImported()
    {
        var source = @"Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim target = New Foo([||],)
        End Sub
    End Class
End Namespace
";
        var newSource = @"Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = NSubstitute.Substitute.For(Of IService)()
            Dim target = New Foo(service,)
        End Sub
    End Class
End Namespace
";
        await VerifyRefactoring(source, newSource, refactoringIndex: 0);
    }

    [Theory]
    [MemberData(nameof(SomeArgumentMissingTestCases))]
    public async Task GeneratesConstructorArgumentListWithLocalVariables_WhenSomeArgumentsMissing(string creationExpression, string expectedCreationExpression)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim someOtherService = Substitute.For(Of IOtherService)()
            Dim someAnotherService = Substitute.For(Of IAnotherService)()
            Dim target = {creationExpression}
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = Substitute.For(Of IService)()
            Dim yetAnotherService = Substitute.For(Of IYetAnotherService)()
            Dim someOtherService = Substitute.For(Of IOtherService)()
            Dim someAnotherService = Substitute.For(Of IAnotherService)()
            Dim target = {expectedCreationExpression}
        End Sub
    End Class
End Namespace";

        await VerifyRefactoring(source, newSource, refactoringIndex: 2);
    }

    [Theory]
    [MemberData(nameof(AllArgumentsMissingTestCases))]
    public async Task GeneratesConstructorArgumentListWithReadonlyFields_WhenAllArgumentsMissing(string creationExpression, string expectedCreationExpression)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim target = {creationExpression}
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Private ReadOnly service As IService = Substitute.For(Of IService)()
        Private ReadOnly otherService As IOtherService = Substitute.For(Of IOtherService)()

        Public Sub Test()
            Dim target = {expectedCreationExpression}
        End Sub
    End Class
End Namespace";

        await VerifyRefactoring(source, newSource, refactoringIndex: 3);
    }

    [Theory]
    [MemberData(nameof(SomeArgumentMissingTestCases))]
    public async Task GeneratesConstructorArgumentListWithReadonlyFields_WhenSomeArgumentsMissing(string creationExpression, string expectedCreationExpression)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim someOtherService = Substitute.For(Of IOtherService)()
            Dim someAnotherService = Substitute.For(Of IAnotherService)()
            Dim target = {creationExpression}
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Private ReadOnly service As IService = Substitute.For(Of IService)()
        Private ReadOnly yetAnotherService As IYetAnotherService = Substitute.For(Of IYetAnotherService)()

        Public Sub Test()
            Dim someOtherService = Substitute.For(Of IOtherService)()
            Dim someAnotherService = Substitute.For(Of IAnotherService)()
            Dim target = {expectedCreationExpression}
        End Sub
    End Class
End Namespace";

        await VerifyRefactoring(source, newSource, refactoringIndex: 3);
    }

    [Theory]
    [MemberData(nameof(SpecificArgumentMissingTestCases))]
    public async Task GeneratesConstructorArgumentListWithLocalVariable_ForSpecificArgument_WhenArgumentMissing(string creationExpression, string expectedCreationExpression)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = Substitute.For(Of IService)()
            Dim someYetAnotherService = Substitute.For(Of IYetAnotherService)()
            Dim target = {creationExpression}
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim otherService = Substitute.For(Of IOtherService)()
            Dim service = Substitute.For(Of IService)()
            Dim someYetAnotherService = Substitute.For(Of IYetAnotherService)()
            Dim target = {expectedCreationExpression}
        End Sub
    End Class
End Namespace";

        await VerifyRefactoring(source, newSource, refactoringIndex: 0);
    }

    [Theory]
    [MemberData(nameof(SpecificArgumentMissingTestCases))]
    public async Task GeneratesConstructorArgumentListWithReadonlyFields_ForSpecificArgument_WhenArgumentMissing(string creationExpression, string expectedCreationExpression)
    {
        var source = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Private ReadOnly service As IService = Substitute.For(Of IService)()
        Private ReadOnly someYetAnotherService As IYetAnotherService = Substitute.For(Of IYetAnotherService)()

        Public Sub Test()
            Dim target = {creationExpression}
        End Sub
    End Class
End Namespace";

        var newSource = $@"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Interface IAnotherService
    End Interface

    Interface IYetAnotherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService, ByVal otherService As IOtherService, ByVal anotherService As IAnotherService, ByVal yetAnotherService As IYetAnotherService)
        End Sub
    End Class

    Public Class FooTests
        Private ReadOnly otherService As IOtherService = Substitute.For(Of IOtherService)()
        Private ReadOnly service As IService = Substitute.For(Of IService)()
        Private ReadOnly someYetAnotherService As IYetAnotherService = Substitute.For(Of IYetAnotherService)()

        Public Sub Test()
            Dim target = {expectedCreationExpression}
        End Sub
    End Class
End Namespace";

        await VerifyRefactoring(source, newSource, refactoringIndex: 1);
    }
}