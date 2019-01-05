using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using NSubstitute.Analyzers.Tests.Shared.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SubstituteAnalyzerTests
{
    public class ForAsNonGenericMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForInterface()
        {
            var source = @"Imports NSubstitute

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

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForInterface_WhenEmptyArrayPassed()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo)}, New Object() {})
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForInterface_AndConstructorParametersUsed()
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
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorArgumentsForInterface,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not provide constructor arguments when substituting for an interface. Use NSubstitute.Substitute.[For]({GetType(IFoo)},Nothing) instead.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(9, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenUsedForDelegate()
        {
            var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Func(Of Integer))}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenUsedForDelegate_AndConstructorParametersUsed()
        {
            var source = @"Imports NSubstitute
Imports System

Namespace MyNamespace
    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Func(Of Integer))}, New Object() {1})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorArgumentsForDelegate,
                Severity = DiagnosticSeverity.Warning,
                Message = "Can not provide constructor arguments when substituting for a delegate. Use NSubstitute.Substitute.[For]({GetType(Func(Of Integer))},Nothing) instead.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(7, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Theory]
        [InlineData("{ GetType(Bar), New Foo().[GetType]() }")]
        [InlineData("{ GetType(Bar), New Foo().[GetType]() }.ToArray()")]
        public async Task ReturnsNoDiagnostic_WhenProxyTypeCannotBeInfered(string proxyExpression)
        {
            var source = $@"Imports NSubstitute
Imports System.Linq

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim bar = New Bar()
            Dim substitute = NSubstitute.Substitute.[For]({proxyExpression}, Nothing)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleTypeParameters_ContainMultipleClasses()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo), GetType(Bar)}, Nothing)
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteMultipleClasses,
                Severity = DiagnosticSeverity.Warning,
                Message =
                    "Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 30)
                }
            };

            await VerifyDiagnostic(source, expectedDiagnostic);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleSameClasses()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo), GetType(Foo)}, Nothing)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsMultipleInterfaces()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Interface IBar
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo), GetType(IBar)}, Nothing)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsNoDiagnostic_WhenMultipleTypeParameters_ContainsInterfaceNotImplementedByClass()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo), GetType(Bar)}, Nothing)
        End Sub
    End Class
End Namespace
";

            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public async Task ReturnsDiagnostic_WhenMultipleTypeParameters_ContainsClassWithoutMatchingConstructor()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class Bar
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(IFoo), GetType(Bar)}, New Object() {1})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Bar. Check the constructors for MyNamespace.Bar and make sure you have passed the required number of arguments.",
                Locations = new[]
                {
                    new DiagnosticResultLocation(12, 30)
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1, 2, 3})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, New Object() {1})
        End Sub
    End Class
End Namespace
";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteForConstructorParametersMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "The number of arguments passed to NSubstitute.Substitute.[For] do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.",
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)
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

<Assembly: InternalsVisibleTo(""OtherFirstAssembly"")>
<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
<Assembly: InternalsVisibleTo(""OtherSecondAssembly"")>
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
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
            Dim substitute = NSubstitute.Substitute.[For]({GetType(Foo)}, Nothing)
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
        [InlineData("ByVal x As Decimal", "New Object() { 1 }")] // valid c# but doesnt work in NSubstitute
        [InlineData("ByVal x As Integer", "New Object() { 1D }")]
        [InlineData("ByVal x As Integer", "New Object() { 1R }")]
        [InlineData("ByVal x As List(Of Integer)", "New Object() { New List(Of Integer)().AsReadOnly() }")]

        // [InlineData("ByVal x As Integer", "New Object()")] This gives runtime error on VB level, not even NSubstitute level (but compiles just fine)
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
            Dim substitute = NSubstitute.Substitute.[For]({{GetType(Foo)}}, {invocationValues})
        End Sub
    End Class
End Namespace";
            var expectedDiagnostic = new DiagnosticResult
            {
                Id = DiagnosticIdentifiers.SubstituteConstructorMismatch,
                Severity = DiagnosticSeverity.Warning,
                Message = "Arguments passed to NSubstitute.Substitute.[For] do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.",
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
            Dim substitute = NSubstitute.Substitute.[For]({{GetType(Foo)}}, {invocationValues})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReturnsNoDiagnostic_WhenUsedWithGenericArgument()
        {
            var source = @"Imports NSubstitute

Namespace MyNamespace
    Public Class FooTests
        Public Function Foo(Of T As Class)() As T
            Return CType(Substitute.[For]({GetType(T)}, Nothing), T)
        End Function
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }
    }
}
