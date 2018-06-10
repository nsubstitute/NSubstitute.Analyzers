using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests
{
    public class ForPartsOfUsedForUnsupportedTypeCodeFixProviderTests : CSharpCodeFixVerifier
    {
        [Fact]
        public async Task ReplacesForPartsOf_WithFor_WhenUsedWithInterface()
        {
            var oldSource = @"using NSubstitute;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.ForPartsOf<IFoo>();
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

            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public async Task ReplacesForPartsOf_WithFor_WhenUsedWithDelegate()
        {
            var oldSource = @"using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.ForPartsOf<System.Func<int>>();
        }
    }
}";
            var newSource = @"using NSubstitute;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = NSubstitute.Substitute.For<System.Func<int>>();
        }
    }
}";

            await VerifyFix(oldSource, newSource);
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new ForPartsOfUsedForUnsupportedTypeCodeFixProvider();
        }
    }
}