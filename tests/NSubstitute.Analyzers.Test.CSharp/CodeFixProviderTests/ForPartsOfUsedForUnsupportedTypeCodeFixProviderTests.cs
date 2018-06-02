using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CodeFixProviders;
using NSubstitute.Analyzers.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Test.CSharp.CodeFixProviderTests
{
    public class ForPartsOfUsedForUnsupportedTypeCodeFixProviderTests : CodeFixProviderTest
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

            await VerifyCSharpFix(oldSource, newSource);
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

            await VerifyCSharpFix(oldSource, newSource);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new ForPartsOfUsedForUnsupportedTypeCodeFixProvider();
        }
    }
}