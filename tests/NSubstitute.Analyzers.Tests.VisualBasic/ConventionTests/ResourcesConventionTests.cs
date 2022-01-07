using NSubstitute.Analyzers.Tests.Shared.Fixtures;
using NSubstitute.Analyzers.VisualBasic;
using Xunit;

namespace NSubstitute.Analyzers.Tests.VisualBasic.ConventionTests;

public class ResourcesConventionTests : IClassFixture<ResourcesConventionFixture>
{
    private readonly ResourcesConventionFixture _resourcesConventionFixture;

    public ResourcesConventionTests(ResourcesConventionFixture resourcesConventionFixture)
    {
        _resourcesConventionFixture = resourcesConventionFixture;
    }

    [Fact]
    public void ResourcesMessageConventionsShouldBeSatisfied()
    {
        _resourcesConventionFixture.AssertDiagnosticDescriptorResourceMessagesFromAssemblyContaining<DiagnosticDescriptorsProvider>();
    }

    [Fact]
    public void ResourcesMessageDuplicationConventionsShouldBeSatisfied()
    {
        _resourcesConventionFixture.AssertDiagnosticDescriptorResourceMessagesDuplicatesFromAssemblyContaining<DiagnosticDescriptorsProvider>();
    }
}