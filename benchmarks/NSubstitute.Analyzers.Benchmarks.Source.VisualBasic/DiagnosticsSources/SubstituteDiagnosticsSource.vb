Imports System
Imports NSubstitute.Analyzers.Benchmarks.Source.VisualBasic.Models
Imports NSubstitute.Core

Namespace DiagnosticsSources
    Public Class SubstituteDiagnosticsSource
        Public Sub NS2000_CanOnlySubstituteForPartsOfClasses()
            Substitute.ForPartsOf(Of IFoo)()
            Substitute.ForPartsOf(Of Action(Of Integer))()
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(IFoo)}, Nothing)
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Action(Of Integer))}, Nothing)
        End Sub

        Public Sub NS2001_CouldNotFindAccessibleConstructor()
            Substitute.ForPartsOf(Of FooWithoutPublicOrProtectedCtor)()
            Substitute.[For](Of FooWithoutPublicOrProtectedCtor)()
            Substitute.[For]({GetType(FooWithoutPublicOrProtectedCtor)}, Nothing)
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(FooWithoutPublicOrProtectedCtor)}, Nothing)
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(FooWithoutPublicOrProtectedCtor)}, Nothing)
        End Sub

        Public Sub NS2002_ConstructorParametersCountMismatch()
            Substitute.[For](Of FooWithoutParameterlessCtor)()
            Substitute.[For](Of FooWithoutParameterlessCtor)(1, 2, 3, 4)
            Substitute.[For]({GetType(FooWithoutParameterlessCtor)}, New Object() {})
            Substitute.[For]({GetType(FooWithoutParameterlessCtor)}, New Object() {1, 2, 3, 4})
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(FooWithoutParameterlessCtor)}, New Object() {})
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(FooWithoutParameterlessCtor)}, New Object() {1, 2, 3, 4})
            Substitute.ForPartsOf(Of FooWithoutParameterlessCtor)()
            Substitute.ForPartsOf(Of FooWithoutParameterlessCtor)(1, 2, 3, 4)
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(FooWithoutParameterlessCtor)}, New Object() {})
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(FooWithoutParameterlessCtor)}, New Object() {1, 2, 3, 4})
        End Sub

        Public Sub NS2003_SubstituteForInternalType()
            Substitute.[For](Of FooInternal)()
            Substitute.[For]({GetType(FooInternal)}, Nothing)
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(FooInternal)}, Nothing)
            Substitute.ForPartsOf(Of FooInternal)()
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(FooInternal)}, Nothing)
        End Sub

        Public Sub NS2004_UnableToFindMatchingConstructor()
            Substitute.[For](Of FooWithoutParameterlessCtor)(1, 1, 1)
            Substitute.[For]({GetType(FooWithoutParameterlessCtor)}, New Object() {1, 1, 1})
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(FooWithoutParameterlessCtor)}, New Object() {1, 1, 1})
            Substitute.ForPartsOf(Of FooWithoutParameterlessCtor)(1, 1, 1)
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(FooWithoutParameterlessCtor)}, New Object() {1, 1, 1})
        End Sub

        Public Sub NS2005_CanNotSubstituteForMultipleClasses()
            Substitute.[For](Of Foo, FooInternal)()
            Substitute.[For]({GetType(Foo), GetType(FooInternal)}, Nothing)
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(Foo), GetType(FooInternal)}, Nothing)
            SubstitutionContext.Current.SubstituteFactory.CreatePartial({GetType(Foo), GetType(FooInternal)}, Nothing)
        End Sub

        Public Sub NS2006_CanNotProvideConstructorArgumentsWhenSubstitutingForAnInterface()
            Substitute.[For](Of IFoo)(1)
            Substitute.[For]({GetType(IFoo)}, New Object() {1})
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(IFoo)}, New Object() {1})
        End Sub

        Public Sub NS2007_CanNotProvideConstructorArgumentsWhenSubstitutingForADelegate()
            Substitute.[For](Of Action)(1)
            Substitute.[For]({GetType(Action)}, New Object() {1})
            SubstitutionContext.Current.SubstituteFactory.Create({GetType(Action)}, New Object() {1})
        End Sub
    End Class
End Namespace
