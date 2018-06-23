using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.ConstructorArgumentsForInterfaceCodeFixProviderTests
{
    public class ConstructorArgumentsForInterfaceCodeFixProviderTests : CSharpCodeFixVerifier
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

            await VerifyFix(source, newSource);
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
            await VerifyFix(source, newSource);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new ConstructorArgumentsForInterfaceCodeFixProvider();
        }
    }
}