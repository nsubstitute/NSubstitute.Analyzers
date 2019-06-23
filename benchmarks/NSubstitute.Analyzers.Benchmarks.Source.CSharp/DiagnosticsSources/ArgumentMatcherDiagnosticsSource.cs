using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class ArgumentMatcherDiagnosticsSource
    {
        public void NS5001_ArgumentMatcherUsedWithoutSpecifyingCall()
        {
            var substitute = Substitute.For<IFoo>();

            substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
            substitute.ObjectReturningMethodWithArguments(Arg.Compat.Any<int>(), Arg.Compat.Any<int>(), Arg.Compat.Any<decimal>());

            substitute.ObjectReturningMethodWithArguments(Arg.Is(0), Arg.Is(0), Arg.Is(0m));
            substitute.ObjectReturningMethodWithArguments(Arg.Compat.Is(0), Arg.Compat.Is(0), Arg.Compat.Is(0m));
            
            // correct usages
            Received.InOrder(() =>
            {
                substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<decimal>());
                substitute.ObjectReturningMethodWithArguments(Arg.Compat.Any<int>(), Arg.Compat.Any<int>(), Arg.Compat.Any<decimal>());

                substitute.ObjectReturningMethodWithArguments(Arg.Is(0), Arg.Is(0), Arg.Is(0m));
                substitute.ObjectReturningMethodWithArguments(Arg.Compat.Is(0), Arg.Compat.Is(0), Arg.Compat.Is(0m));
            });
        }
    }
}