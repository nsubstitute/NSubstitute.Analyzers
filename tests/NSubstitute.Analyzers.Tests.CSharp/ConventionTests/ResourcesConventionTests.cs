using NSubstitute.Analyzers.CSharp;
using NSubstitute.Analyzers.Tests.Shared.Fixtures;
using Xunit;

namespace NSubstitute.Analyzers.Tests.CSharp.ConventionTests
{
    public class ResourcesConventionTests : IClassFixture<ResourcesConventionFixture>
    {
        private readonly ResourcesConventionFixture _resourcesConventionFixture;

        public ResourcesConventionTests(ResourcesConventionFixture resourcesConventionFixture)
        {
            _resourcesConventionFixture = resourcesConventionFixture;
        }

        [Fact]
        public void ResourcesConventionsShouldBeSatisfied()
        {
            _resourcesConventionFixture.AssertDiagnosticDescriptorResourceMessagesFromAssemblyContaining<DiagnosticDescriptorsProvider>();
        }

        [Fact]
        public void ResourcesMessageDuplicationConventionsShouldBeSatisfied()
        {
            _resourcesConventionFixture.AssertDiagnosticDescriptorResourceMessagesDuplicatesFromAssemblyContaining<DiagnosticDescriptorsProvider>();
        }
    }
}