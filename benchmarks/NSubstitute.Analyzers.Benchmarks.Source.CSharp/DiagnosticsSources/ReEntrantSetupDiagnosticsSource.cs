using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;
using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class ReEntrantSetupDiagnosticsSource
    {
        public void NS4000_ReEntrantSubstituteCall()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.CallInfoReturningMethod().Returns(Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null));
            substitute.ConfiguredCallReturningProperty.Returns(Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null));
            substitute[0, 0, 0, 0].Returns(Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null));
            substitute.CallInfoReturningMethod().Returns(ReEntrantReturn());
            substitute.ConfiguredCallReturningProperty.Returns(ReEntrantReturn());
            substitute[0, 0, 0, 0].Returns(ReEntrantReturn());

            SubstituteExtensions.Returns(substitute.CallInfoReturningMethod(), Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null));
            SubstituteExtensions.Returns(substitute.ConfiguredCallReturningProperty, Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null));
            SubstituteExtensions.Returns(substitute[0, 0, 0, 0], Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null));
            SubstituteExtensions.Returns(substitute.CallInfoReturningMethod(), ReEntrantReturn());
            SubstituteExtensions.Returns(substitute.ConfiguredCallReturningProperty, ReEntrantReturn());
            SubstituteExtensions.Returns(substitute[0, 0, 0, 0], ReEntrantReturn());
        }

        private static ConfiguredCall ReEntrantReturn()
        {
            return Substitute.For<IFoo>().ObjectReturningMethod().Returns((IFoo)null);
        }
    }
}