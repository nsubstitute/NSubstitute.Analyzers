using NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models;
using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.DiagnosticsSources
{
    public class ReEntrantSetupDiagnosticsSource
    {
        public void NS4000_ReEntrantSubstituteCall()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethod().Returns(Substitute.For<IFoo>().ObjectReturningMethod().Returns(1));
            substitute.ObjectReturningProperty.Returns(Substitute.For<IFoo>().ObjectReturningMethod().Returns(1));
            substitute[0, 0].Returns(Substitute.For<IFoo>().ObjectReturningMethod().Returns(1));
            substitute.ObjectReturningMethod().Returns(ReEntrantReturn());
            substitute.ObjectReturningProperty.Returns(ReEntrantReturn());
            substitute[0, 0].Returns(ReEntrantReturn());
            
            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), Substitute.For<IFoo>().ObjectReturningMethod().Returns(1));
            SubstituteExtensions.Returns(substitute.ObjectReturningProperty, Substitute.For<IFoo>().ObjectReturningMethod().Returns(1));
            SubstituteExtensions.Returns(substitute[0, 0], Substitute.For<IFoo>().ObjectReturningMethod().Returns(1));
            SubstituteExtensions.Returns(substitute.ObjectReturningMethod(), ReEntrantReturn());
            SubstituteExtensions.Returns(substitute.ObjectReturningProperty, ReEntrantReturn());
            SubstituteExtensions.Returns(substitute[0, 0], ReEntrantReturn());
        }

        private static ConfiguredCall ReEntrantReturn()
        {
            return Substitute.For<IFoo>().ObjectReturningMethod().Returns(1);
        }
    }
}