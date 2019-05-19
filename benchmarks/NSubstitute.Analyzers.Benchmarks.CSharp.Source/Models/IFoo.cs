namespace NSubstitute.Analyzers.Benchmarks.CSharp.Source.Models
{
    public interface IFoo
    {
        void VoidReturningMethod();

        object ObjectReturningMethod();
        
        void VoidReturningMethodWithArguments(int x, int y, decimal z);

        object ObjectReturningMethodWithArguments(int x, int y, decimal z);
        
        object ObjectReturningMethodWithRefArguments(ref int x, ref int y, ref decimal z);
        
        int Property { get; }
        
        object ObjectReturningProperty { get; }
        
        int this[int i] { get; }
        
        object this[int i, int y] { get; }
    }
}