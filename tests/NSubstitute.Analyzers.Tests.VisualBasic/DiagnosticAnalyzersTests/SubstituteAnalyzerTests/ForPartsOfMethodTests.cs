using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SubstituteAnalyzerTests
{
    public class ForPartsOfMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForInterface()
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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can only substitute for parts of classes, not interfaces or delegates. Use NSubstitute.Substitute.For(Of IFoo)() instead of NSubstitute.Substitute.ForPartsOf(Of IFoo)() here.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForDelegate()
        {
            var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Func(Of Integer))()
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForPartsOfUsedForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can only substitute for parts of classes, not interfaces or delegates. Use NSubstitute.Substitute.For(Of Func(Of Integer))() instead of NSubstitute.Substitute.ForPartsOf(Of Func(Of Integer))() here.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(10, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReturnsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Private Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForWithoutAccessibleConstructor,
                Severity = DiagnosticSeverity.Warning,
                Message = "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(11, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)(1, 2, 3)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(11, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReturnsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)(1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(11, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReturnsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal Optional y As Integer = 1)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)(1)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(11, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForInternalMember,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Can not substitute for internal type. To substitute for internal type expose your type to DynamicProxyGenAssembly2 via <Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        public override async Task ReturnsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReturnsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly()
        {
            var source = @"Imports NSubstitute
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""SomeValue"")>
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)()
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForInternalMember,
                Severity = DiagnosticSeverity.Warning,
                Message = @"Can not substitute for internal type. To substitute for internal type expose your type to DynamicProxyGenAssembly2 via <Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>",
                Locations = new[]
                {
                    new DiagnosticResultLocation(11, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("ByVal x As Decimal", "1")] // valid c# but doesnt work in NSubstitute
        [InlineData("ByVal x As Integer", "1D")]
        [InlineData("ByVal x As Integer", "1R")]
        [InlineData("ByVal x As List(Of Integer)", "New List(Of Integer)().AsReadOnly()")]
        [InlineData("ByVal x As Integer", "New Object()")]
        public override async Task ReturnsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"Imports NSubstitute
Imports System.Collections.Generic

Namespace MyNamespace
    Public Class Foo
        Public Sub New({ctorValues})
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)({invocationValues})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Arguments passed to NSubstitute.Substitute.ForPartsOf(Of MyNamespace.Foo) do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("ByVal x As Integer", "New Object() {1}")]
        [InlineData("ByVal x As Single", "New Object() {\"c\"c}")]
        [InlineData("ByVal x As Integer", "New Object() {\"c\"c}")]
        [InlineData("ByVal x As IList(Of Integer)", "New Object() {New List(Of Integer)()}")]
        [InlineData("ByVal x As IEnumerable(Of Integer)", "New Object() {New List(Of Integer)()}")]
        [InlineData("ByVal x As IEnumerable(Of Integer)", "New Object() {New List(Of Integer)().AsReadOnly()}")]
        [InlineData("ByVal x As IEnumerable(Of Char)", @"New Object() {""value""}")]
        [InlineData("", @"New Object() {}")]
        [InlineData("", "New Object() {1, 2}.ToArray()")] // actual values known at runtime only so constructor analysys skipped
        public override async Task ReturnsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"Imports NSubstitute
Imports System.Collections.Generic
Imports System.Linq

Namespace MyNamespace
    Public Class Foo
        Public Sub New({ctorValues})
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.ForPartsOf(Of Foo)({invocationValues})
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }

        public override async Task ReturnsNoDiagnostic_WhenUsedWithGenericArgument()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Function FooPartsOf(Of T As Class)() As T
            Return Substitute.ForPartsOf(Of T)()
        End Function
    End Class
End Namespace
";
            await VerifyDiagnostic(source);
        }
    }
}