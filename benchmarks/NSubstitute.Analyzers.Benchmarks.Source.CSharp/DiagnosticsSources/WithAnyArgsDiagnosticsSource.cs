using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class WithAnyArgsDiagnosticsSource
    {
        public void NS5004_InvalidArgumentMatcherUsedWithAnyArgs()
        {
            var substitute = Substitute.For<IFoo>();

            _ = substitute.DidNotReceiveWithAnyArgs()[Arg.Is(1)];
            _ = substitute.DidNotReceiveWithAnyArgs()[Arg.Do<int>(_ => { })];
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.Is(1);
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.Do<int>(_ => { });
            substitute.DidNotReceiveWithAnyArgs()
                .ObjectReturningMethodWithArguments(Arg.Is(1), Arg.Is(1), Arg.Do<int>(_ => { }));

            _ = substitute.DidNotReceiveWithAnyArgs()[Arg.Is(1)];
            _ = substitute.DidNotReceiveWithAnyArgs()[Arg.Do<int>(_ => { })];
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.Is(1);
            substitute.DidNotReceiveWithAnyArgs().IntReturningProperty = Arg.Do<int>(_ => { });
            substitute.DidNotReceiveWithAnyArgs()
                .ObjectReturningMethodWithArguments(Arg.Is(1), Arg.Is(1), Arg.Do<int>(_ => { }));

            _ = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)[Arg.Is(1)];
            _ = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)[Arg.Do<int>(_ => { })];
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.Is(1);
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.Do<int>(_ => { });
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)
                .ObjectReturningMethodWithArguments(Arg.Is(1), Arg.Is(1), Arg.Do<int>(_ => { }));

            _ = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)[Arg.Is(1)];
            _ = SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)[Arg.Do<int>(_ => { })];
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.Is(1);
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute).IntReturningProperty = Arg.Do<int>(_ => { });
            SubstituteExtensions.DidNotReceiveWithAnyArgs(substitute)
                .ObjectReturningMethodWithArguments(Arg.Is(1), Arg.Is(1), Arg.Do<int>(_ => { }));
        }
    }
}
