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
            var exportedTypes = assemblies.SelectMany(assembly => assembly.GetExportedTypes())
                .Where(type => IsInstrumentationType(type) == false);

            exportedTypes.Should().BeEmpty("because all types should be internal");
        }

        private bool IsInstrumentationType(Type type)
        {
            // coverlet dynamically create type used for calculating coverage
            var typeName = type.Name;
            var underscoreIndex = typeName.LastIndexOf('_');
            if (underscoreIndex < 0)
            {
                return false;
            }

            var typeSuffix = typeName.Substring(underscoreIndex + 1, typeName.Length - underscoreIndex - 1);

            return Guid.TryParse(typeSuffix, out _);
        }
    }
}