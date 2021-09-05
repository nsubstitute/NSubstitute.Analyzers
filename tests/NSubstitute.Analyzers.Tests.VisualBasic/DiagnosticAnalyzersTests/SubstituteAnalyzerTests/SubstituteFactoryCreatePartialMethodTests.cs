using System.Threading.Tasks;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.DiagnosticAnalyzersTests.SubstituteAnalyzerTests
{
    public class SubstituteFactoryCreatePartialMethodTests : SubstituteDiagnosticVerifier
    {
        [Fact]
        public async Task ReportsDiagnostic_WhenUsedForInterface()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(IFoo)}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use SubstitutionContext.Current.SubstituteFactory.Create(New Type() {GetType(IFoo)}, Nothing) instead of SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(IFoo)}, Nothing) here.");
        }

        [Fact]
        public async Task ReportsDiagnostic_WhenUsedForDelegate()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Interface IFoo
    End Interface

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Func(Of Integer))}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, PartialSubstituteForUnsupportedTypeDescriptor, "Can only substitute for parts of classes, not interfaces or delegates. Use SubstitutionContext.Current.SubstituteFactory.Create(New Type() {GetType(Func(Of Integer))}, Nothing) instead of SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Func(Of Integer))}, Nothing) here.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenUsedForClassWithoutPublicOrProtectedConstructor()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Private Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        public override async Task ReportsDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToNotApplied()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        public override async Task ReportsDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToNotApplied()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Protected Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForWithoutAccessibleConstructorDescriptor, "Could not find accessible constructor. Make sure that type MyNamespace.Foo exposes public or protected constructors.");
        }

        public override async Task ReportsNoDiagnostic_WhenUsedForClassWithInternalConstructor_AndInternalsVisibleToApplied()
        {
            var source = @"Imports System
Imports NSubstitute.Core
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Public Class Foo
        Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedForClassWithProtectedInternalConstructor_AndInternalsVisibleToApplied()
        {
            var source = @"Imports System
Imports NSubstitute.Core
Imports System.Runtime.CompilerServices

<Assembly: InternalsVisibleTo(""DynamicProxyGenAssembly2"")>
Namespace MyNamespace
    Public Class Foo
        Protected Friend Sub New()
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenPassedParametersCount_GreaterThanCtorParametersCount()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, New Object() {1, 2, 3})|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, New Object() {1})|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenUsedWithWithoutProvidingOptionalParameters()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal Optional y As Integer = 1)
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, New Object() {1})|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }

        [Fact]
        public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToNotApplied()
        {
            var source = @"Imports System
Imports NSubstitute.Core

Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(Foo)}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToDynamicProxyGenAssembly2(string assemblyAttributes)
        {
            var source = $@"Imports System
Imports System.Runtime.CompilerServices
Imports NSubstitute.Core

{assemblyAttributes}
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {{GetType(Foo)}}, Nothing)
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenUsedWithInternalClass_AndInternalsVisibleToAppliedToWrongAssembly(string assemblyAttributes)
        {
            var source = $@"Imports System
Imports System.Runtime.CompilerServices
Imports NSubstitute.Core

{assemblyAttributes}
Namespace MyNamespace
    Friend Class Foo
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {{GetType(Foo)}}, Nothing)|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForInternalMemberDescriptor);
        }

        [Theory]
        [InlineData("ByVal x As Decimal", "New Object() { 1 }")] // valid c# but doesnt work in NSubstitute
        [InlineData("ByVal x As Integer", "New Object() { 1D }")]
        [InlineData("ByVal x As Integer", "New Object() { 1R }")]
        [InlineData("ByVal x As List(Of Integer)", "New Object() { New List(Of Integer)().AsReadOnly() }")]
        [InlineData("ParamArray z As Integer()", "New Object() { 1D }")]

        // [InlineData("ByVal x As Integer", "New Object()")] This gives runtime error on VB level, not even NSubstitute level (but compiles just fine)
        public override async Task ReportsDiagnostic_WhenConstructorArgumentsRequireExplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"Imports NSubstitute
Imports System.Collections.Generic
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New({ctorValues})
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial({{GetType(Foo)}}, {invocationValues})|]
        End Sub
    End Class
End Namespace";

            await VerifyDiagnostic(source, SubstituteConstructorMismatchDescriptor, "Arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required arguments and argument types.");
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
        [InlineData("ByVal x As Integer, ParamArray z As String()", "New Object() { 1, \"foo\" }")]
        public override async Task ReportsNoDiagnostic_WhenConstructorArgumentsDoNotRequireImplicitConversion(string ctorValues, string invocationValues)
        {
            var source = $@"Imports NSubstitute
Imports System.Collections.Generic
Imports System.Linq
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New({ctorValues})
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({{GetType(Foo)}}, {invocationValues})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenUsedWithGenericArgument()
        {
            var source = @"Imports System
Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class FooTests
        Public Function Foo(Of T As Class)() As T
            Return CType(SubstitutionContext.Current.SubstituteFactory.CreatePartial(New Type() {GetType(T)}, Nothing), T)
        End Function
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenParamsParametersNotProvided()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ParamArray y As Object())
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Foo)}, New Object() {1})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsNoDiagnostic_WhenParamsParametersProvided()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ParamArray y As Object())
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Foo)}, New Object() {1, 2, 3})
        End Sub
    End Class
End Namespace
";
            await VerifyNoDiagnostic(source);
        }

        public override async Task ReportsDiagnostic_WhenPassedParametersCount_LessThanCtorParametersCount_AndParamsParameterDefined()
        {
            var source = @"Imports NSubstitute
Imports NSubstitute.Core

Namespace MyNamespace
    Public Class Foo
        Public Sub New(ByVal x As Integer, ByVal y As Integer, ParamArray z As Object())
        End Sub
    End Class

    Public Class FooTests
        Public Sub Test()
            Dim substitute = [|SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Foo)}, New Object() {1})|]
        End Sub
    End Class
End Namespace
";
            await VerifyDiagnostic(source, SubstituteForConstructorParametersMismatchDescriptor, "The number of arguments passed to NSubstitute.Core.ISubstituteFactory.CreatePartial do not match the number of constructor arguments for MyNamespace.Foo. Check the constructors for MyNamespace.Foo and make sure you have passed the required number of arguments.");
        }
    }
}