using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models
{
    public interface IFoo
    {
        void VoidReturningMethod();

        IFoo ObjectReturningMethod();

        ConfiguredCall CallInfoReturningMethod();

        ConfiguredCall ConfiguredCallReturningProperty { get; }

        ConfiguredCall this[int a, int b, int c, int d] { get; }

        void VoidReturningMethodWithArguments(int x, int y, decimal z);

        IFoo ObjectReturningMethodWithArguments(int x, int y, decimal z);

        IFoo ObjectReturningMethodWithRefArguments(ref int x, ref int y, ref decimal z);

        int Property { get; }

        IFoo ObjectReturningProperty { get; }

        int this[int i] { get; }

        IFoo this[int i, int y] { get; }
    }
}