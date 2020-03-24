using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using NSubstitute.Analyzers.Tests.Shared.CodeRefactoringProviders;
using NSubstitute.Analyzers.VisualBasic.CodeRefactoringProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeRefactoringProvidersTests.IntroduceSubstituteRefactoringProviderTests
{
    public class IntroduceSubstituteCodeRefactoringActionsTests : VisualBasicCodeRefactoringProviderActionsVerifier, IIntroduceSubstituteCodeRefactoringActionsVerifier
    {
        private static readonly string IntroduceReadonlySubstitutesTitle = "Introduce readonly substitutes for missing arguments";
        private static readonly string IntroduceLocalSubstitutesTitle = "Introduce local substitutes for missing arguments";
        private static readonly string IntroduceReadonlySubstituteForService = "Introduce readonly substitute for MyNamespace.IService";

        protected override CodeRefactoringProvider CodeRefactoringProvider { get; } = new IntroduceSubstituteCodeRefactoringProvider();

        [Fact]
        public async Task DoesNotCreateCodeActions_WhenConstructorHasNoParameters()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim target = New Foo([||])
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Fact]
        public async Task DoesNotCreateCodeActions_WhenMultipleCandidateConstructorsAvailable()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService)
        End Sub

        Public Sub New(ByVal otherService As IOtherService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim target = New Foo([||])
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Fact]
        public async Task DoesNotCreateCodeActions_WhenAllParametersProvided()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Public Class Foo
        Public Sub New(ByVal service As IService)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = Substitute.[For](Of IService)()
            Dim target = New Foo([||]service)
            service
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, Array.Empty<string>());
        }

        [Fact]
        public async Task DoesNotCreateCodeActionsForIntroducingSpecificSubstitute_WhenArgumentAtPositionProvided()
        {
            var source = @"Imports NSubstitute

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
            Dim service = Substitute.[For](Of IService)()
            Dim target = New Foo(service[||],)
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, IntroduceLocalSubstitutesTitle, IntroduceReadonlySubstitutesTitle);
        }

        [Fact]
        public async Task DoesNotCreateCodeActionsForIntroducingLocalSubstitute_WhenLocalVariableCannotBeIntroduced()
        {
            var source = @"Imports NSubstitute

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
        Private ReadOnly foo As Foo = New Foo([||])
    End Class
End Namespace";

            await VerifyCodeActions(source, IntroduceReadonlySubstituteForService, IntroduceReadonlySubstitutesTitle);
        }

        [Theory]
        [InlineData("New Foo([||],)", new[] { "Introduce local substitute for MyNamespace.IService", "Introduce readonly substitute for MyNamespace.IService" })]
        [InlineData("New Foo(,[||])", new[] { "Introduce local substitute for MyNamespace.IOtherService", "Introduce readonly substitute for MyNamespace.IOtherService" })]
        public async Task CreatesCodeActionsForIntroducingSpecificSubstitute_WhenArgumentAtPositionNotProvided(string creationExpression, string[] expectedSpecificSubstituteActions)
        {
            var expectedAllActions = new[]
            {
                IntroduceLocalSubstitutesTitle,
                IntroduceReadonlySubstitutesTitle
            }.Concat(expectedSpecificSubstituteActions).ToArray();

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

            await VerifyCodeActions(source, expectedAllActions);
        }

        [Fact]
        public async Task DoesNotCreateCodeActions_WhenSymbolIsNotConstructorInvocation()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Interface IService
    End Interface

    Interface IOtherService
    End Interface

    Public Class Foo
        Public Sub New([||]ByVal service As IService, [||]ByVal otherService As IIOtherService)
        End Sub

        Public Sub New(ByVal x As Integer, [||])
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim service = Substitute.[For](Of IService)([||])
            Dim x = FooBar(1, [||])
            Dim z = New Integer() {[||]}
        End Sub

        Private Sub FooBar([||]ByVal x As Integer, [||]ByVal y As Integer)
        End Sub
    End Class
End Namespace";

            await VerifyCodeActions(source, Array.Empty<string>());
        }
    }
}