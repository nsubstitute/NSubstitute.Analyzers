using System;
using NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models;
using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class SubstituteDiagnosticsSource
    {
        public void NS2000_CanOnlySubstituteForPartsOfClasses()
        {
            Substitute.ForPartsOf<IFoo>();
            Substitute.ForPartsOf<Action<int>>();
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(IFoo)}, null);
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(Action<int>)}, null);
        }

        public void NS2001_CouldNotFindAccessibleConstructor()
        {
            Substitute.ForPartsOf<FooWithoutPublicOrProtectedCtor>();
            Substitute.For<FooWithoutPublicOrProtectedCtor>();
            Substitute.For(new[] {typeof(FooWithoutPublicOrProtectedCtor)}, null);
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(FooWithoutPublicOrProtectedCtor)}, null);
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(FooWithoutPublicOrProtectedCtor)}, null);
        }

        public void NS2002_ConstructorParametersCountMismatch()
        {
            Substitute.For<FooWithoutParameterlessCtor>();
            Substitute.For<FooWithoutParameterlessCtor>(1, 2, 3, 4);
            Substitute.For(new[] {typeof(FooWithoutParameterlessCtor)}, new object[] { });
            Substitute.For(new[] {typeof(FooWithoutParameterlessCtor)}, new object[] { 1, 2, 3, 4 });
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(FooWithoutParameterlessCtor)}, new object[]{ });
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(FooWithoutParameterlessCtor)}, new object[] { 1, 2, 3, 4 });
            
            Substitute.ForPartsOf<FooWithoutParameterlessCtor>();
            Substitute.ForPartsOf<FooWithoutParameterlessCtor>(1, 2, 3, 4);
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(FooWithoutParameterlessCtor)}, new object[]{ });
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(FooWithoutParameterlessCtor)}, new object[] { 1, 2, 3, 4 });

        }

        public void NS2003_SubstituteForInternalType()
        {
            Substitute.For<FooInternal>();
            Substitute.For(new[] {typeof(FooInternal)}, null);
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(FooInternal)}, null);
            
            Substitute.ForPartsOf<FooInternal>();
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(FooInternal)}, null);
        }

        public void NS2004_UnableToFindMatchingConstructor()
        {
            Substitute.For<FooWithoutParameterlessCtor>(1, 1, 1);
            Substitute.For(new [] {typeof(FooWithoutParameterlessCtor)}, new object[] { 1, 1, 1 });
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(FooWithoutParameterlessCtor)}, new object[] { 1, 1, 1 });

            Substitute.ForPartsOf<FooWithoutParameterlessCtor>(1, 1, 1);
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(FooWithoutParameterlessCtor)}, new object[] { 1, 1, 1 });
        }

        public void NS2005_CanNotSubstituteForMultipleClasses()
        {
            Substitute.For<Foo, FooInternal>();
            Substitute.For(new [] { typeof(Foo), typeof(FooInternal)}, null);
            SubstitutionContext.Current.SubstituteFactory.Create(new [] { typeof(Foo), typeof(FooInternal)}, null);
            
            SubstitutionContext.Current.SubstituteFactory.CreatePartial(new [] { typeof(Foo), typeof(FooInternal)}, null);
        }

        public void NS2006_CanNotProvideConstructorArgumentsWhenSubstitutingForAnInterface()
        {
            Substitute.For<IFoo>(1);
            Substitute.For(new [] { typeof(IFoo)}, new object[] { 1 });
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, new object[] { 1 });
        }
        
        public void NS2007_CanNotProvideConstructorArgumentsWhenSubstitutingForADelegate()
        {
            Substitute.For<Action>(1);
            Substitute.For(new [] { typeof(Action)}, new object[] { 1 });
            SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(Action)}, new object[] { 1 });
        }
    }
}