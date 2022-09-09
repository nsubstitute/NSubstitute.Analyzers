using NSubstitute.Core;

namespace NSubstitute.Analyzers.Benchmarks.Source.CSharp.Models
{
    public interface IFoo
    {
        void VoidReturningMethod();

        void VoidReturningMethodWithArguments(int a, int b, decimal c);

        IFoo ObjectReturningMethod();

        IFoo ObjectReturningMethodWithArguments(int a, int b, decimal c);

        IFoo ObjectReturningMethodWithRefArguments(ref int a, ref int b, ref decimal c);

        ConfiguredCall CallInfoReturningMethod();

        ConfiguredCall ConfiguredCallReturningProperty { get; }

        int IntReturningProperty { get; set; }

        IFoo ObjectReturningProperty { get; }

        int this[int a] { get; }

        IFoo this[int a, int b] { get; }

        ConfiguredCall this[int a, int b, int c, int d] { get; }
    }
}