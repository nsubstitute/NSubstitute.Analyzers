using NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.DiagnosticsSources
{
    public class NonSubstitutableMemberArgumentMatcherDiagnosticsSource
    {
        public void NS1004_ArgumentMatcherUsedWithNonVirtualMember()
        {
            var substitute = Substitute.For<Foo>();
            substitute.ObjectReturningMethodWithArguments(Arg.Any<int>(), Arg.Compat.Any<int>(), Arg.Is(1m));
            substitute.ObjectReturningMethodWithArguments(Arg.Do<int>(i => { }), Arg.Compat.Do<int>(i => { }), Arg.Compat.Is(1m));
            substitute.ObjectReturningMethodWithArguments(Arg.Invoke(1), Arg.InvokeDelegate<int>(), Arg.Compat.InvokeDelegate<int>());
            substitute.ObjectReturningMethodWithArguments(Arg.Compat.Invoke(1), Arg.Compat.Do<int>(i => { }), Arg.Compat.Is(1m));
            substitute.InternalObjectReturningMethodWithArguments(Arg.Is(1));
            substitute.InternalObjectReturningMethodWithArguments(Arg.Compat.Is(1));
            substitute.InternalObjectReturningMethodWithArguments(Arg.Any<int>());
            substitute.InternalObjectReturningMethodWithArguments(Arg.Compat.Any<int>());
            _ = substitute[Arg.Any<int>()];
            _ = substitute[Arg.Compat.Any<int>()];
        }
    }
}
