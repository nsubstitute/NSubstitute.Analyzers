using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.PartialSubstituteUsedForUnsupportedTypeCodeFixProviderTests
{
    public class PartialSubstituteUsedForUnsupportedTypeCodeFixProviderTests : CSharpCodeFixVerifier, IForPartsOfUsedForUnsupportedTypeCodeFixVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer { get; } = new SubstituteAnalyzer();

        protected override CodeFixProvider CodeFixProvider { get; } = new PartialSubstituteUsedForUnsupportedTypeCodeFixProvider();

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

        [Fact]
        public async Task ReplacesSubstituteFactoryCreatePartial_WithSubstituteFactoryCreate_WhenUsedWithDelegate()
        {
            var oldSource = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(System.Func<int>)}, null);
        }
    }
}";
            var newSource = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(System.Func<int>)}, null);
        }
    }
}";

            await VerifyFix(oldSource, newSource);
        }

        [Fact]
        public async Task ReplacesSubstituteFactoryCreatePartial_WithSubstituteFactoryCreate_WhenUsedWithInterface()
        {
            var oldSource = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.CreatePartial(new[] {typeof(IFoo)}, null);
        }
    }
}";
            var newSource = @"using NSubstitute;
using NSubstitute.Core;

namespace MyNamespace
{
    public interface IFoo
    {
    }

    public class FooTests
    {
        public void Test()
        {
            var substitute = SubstitutionContext.Current.SubstituteFactory.Create(new[] {typeof(IFoo)}, null);
        }
    }
}";

            await VerifyFix(oldSource, newSource);
        }
    }
}