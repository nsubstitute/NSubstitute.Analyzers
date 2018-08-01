using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.CSharp.CodeFixProviders;
using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.CodeFixProviderTests.PartialSubstituteUsedForUnsupportedTypeCodeFixProviderTests
{
    public class PartialSubstituteUsedForUnsupportedTypeCodeFixActionsTests : CSharpCodeFixActionsVerifier, IPartialSubstituteUsedForUnsupportedTypeCodeFixActionsVerifier
    {
        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForSubstituteForPartsOf()
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
            var substitute = NSubstitute.Substitute.ForPartsOf<IFoo>();
        }
    }
}";
            await VerifyCodeActions(source, "Use Substitute.For");
        }

        [Fact]
        public async Task CreatesCorrectCodeFixActions_ForSubstituteFactoryCreatePartial()
        {
            var source = @"using NSubstitute;
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
            await VerifyCodeActions(source, "Use SubstituteFactory.Create");
        }

        protected override DiagnosticAnalyzer GetDiagnosticAnalyzer()
        {
            return new SubstituteAnalyzer();
        }

        protected override CodeFixProvider GetCodeFixProvider()
        {
            return new PartialSubstituteUsedForUnsupportedTypeCodeFixProvider();
        }
    }
}