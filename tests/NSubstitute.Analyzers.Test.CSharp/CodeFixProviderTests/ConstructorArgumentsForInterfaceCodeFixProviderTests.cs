using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CodeFixProviders;
using NSubstitute.Analyzers.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.CodeFixProviderTests
{
    public class ConstructorArgumentsForInterfaceCodeFixProviderTests : CodeFixProviderTest
    {
        [Fact]
        public async Task RemovesInvocationArguments_WhenGenericFor_UsedWithParametersForInterface()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>(1, 2, 3);
        }
    }
}";
            var newSource = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<IFoo>();
        }
    }
}";

            await VerifyCSharpFix(source, newSource);
        }

        [Fact]
        public async Task RemovesInvocationArguments_WhenNonGenericFor_UsedWithParametersForInterface()
        {
            var source = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, new object[] { 1 });
        }
    }
}";
            var newSource = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For(new [] { typeof(IFoo) }, null);
        }
    }
}";
            await VerifyCSharpFix(source, newSource);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ConstructorArgumentsForInterfaceCodeFixProvider();
        }
    }
}