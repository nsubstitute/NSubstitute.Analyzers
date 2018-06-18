using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;

namespace NSubstitute.Analyzers.Tests.Shared.Fixtures
{
    public class TypeVisibilityConventionFixture
    {
        public void AssertTypeVisibilityConventionsFromAssemblyContaining<T>()
        {
            AssertTypeVisibilityConventionsFromAssemblyContaining(typeof(T));
        }

        public void AssertTypeVisibilityConventionsFromAssemblyContaining(Type type)
        {
            AssertTypeVisibilityConventionsFromAssembly(type.Assembly);
        }

        public void AssertTypeVisibilityConventionsFromAssembly(params Assembly[] assemblies)
        {
            assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
            var exportedTypes = assemblies.SelectMany(assembly => assembly.GetExportedTypes());

            exportedTypes.Should().BeEmpty("because all types should be internal");
        }
    }
}