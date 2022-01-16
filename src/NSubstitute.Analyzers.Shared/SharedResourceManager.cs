using System.Reflection;
using System.Resources;

namespace NSubstitute.Analyzers.Shared;

internal class SharedResourceManager
{
    internal static ResourceManager Instance { get; } = new ResourceManager(
        $"{typeof(SharedResourceManager).GetTypeInfo().Assembly.GetName().Name}.Resources",
        typeof(SharedResourceManager).GetTypeInfo().Assembly);
}