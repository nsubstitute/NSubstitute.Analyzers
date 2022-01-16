using NSubstitute.Analyzers.Tests.Shared.Fixtures;
using NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.ConventionTests;

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
        _typeVisibilityConventionFixture.AssertTypeVisibilityConventionsFromAssemblyContaining<NonSubstitutableMemberAnalyzer>();
    }
}