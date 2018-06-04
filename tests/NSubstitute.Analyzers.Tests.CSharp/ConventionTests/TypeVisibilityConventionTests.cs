using NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Tests.Shared;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.ConventionTests
{
    public class TypeVisibilityConventionTests : IClassFixture<TypeVisibilityConventionFixture>
    {
        private readonly TypeVisibilityConventionFixture _typeVisibilityConventionFixture;

        public TypeVisibilityConventionTests(TypeVisibilityConventionFixture typeVisibilityConventionFixture)
        {
            _typeVisibilityConventionFixture = typeVisibilityConventionFixture;
        }

        [Fact]
        public void TypeVisibilityConventionsShouldBeSatisfied()
        {
            _typeVisibilityConventionFixture.AssertTypeVisibilityConventionsFromAssembly(
                typeof(NonVirtualSetupAnalyzer).Assembly, typeof(AbstractDiagnosticDescriptorsProvider<>).Assembly);
        }
    }
}