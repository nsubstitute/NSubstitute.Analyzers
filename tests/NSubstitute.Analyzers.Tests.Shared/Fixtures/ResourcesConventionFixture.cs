using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.Tests.Shared.Fixtures;

public class ResourcesConventionFixture
{
    private static readonly string[] ResourceEntryNameSuffixes =
    {
        nameof(DiagnosticDescriptor.Title),
        nameof(DiagnosticDescriptor.MessageFormat),
        nameof(DiagnosticDescriptor.Description)
    };

    public void AssertDiagnosticDescriptorResourceMessagesFromAssemblyContaining<T>()
    {
        var specificResourceManager = GetSpecificResourceManager<T>();

        var missingResources = GetResourceEntryNames()
            .Where(entryName => HasResourceEntry(specificResourceManager, SharedResourceManager.Instance, entryName) == false);

        missingResources.Should().BeEmpty("because every diagnostic should have diagnostic message specified for Title, MessageFormat and Description");
    }

    public void AssertDiagnosticDescriptorResourceMessagesDuplicatesFromAssemblyContaining<T>()
    {
        var specificResourceManager = new ResourceManager(
            $"{typeof(T).GetTypeInfo().Assembly.GetName().Name}.Resources",
            typeof(T).GetTypeInfo().Assembly);

        var duplicateEntries = GetResourceEntryNames()
            .Where(entryName => HasResourceEntry(specificResourceManager, entryName) &&
                                HasResourceEntry(SharedResourceManager.Instance, entryName));

        duplicateEntries.Should().BeEmpty("because every diagnostic should be defined only in one resource manager");
    }

    private static ResourceManager GetSpecificResourceManager<T>()
    {
        var specificResourceManager = new ResourceManager(
            $"{typeof(T).GetTypeInfo().Assembly.GetName().Name}.Resources",
            typeof(T).GetTypeInfo().Assembly);
        return specificResourceManager;
    }

    private static bool HasResourceEntry(ResourceManager specificResourceManager, string entryName)
    {
        return string.IsNullOrEmpty(specificResourceManager.GetString(entryName)) == false;
    }

    private static bool HasResourceEntry(ResourceManager specificResourceManager, ResourceManager sharedResourceManager, string entryName)
    {
        return HasResourceEntry(specificResourceManager, entryName) || HasResourceEntry(sharedResourceManager, entryName);
    }

    private static IEnumerable<string> GetResourceEntryNames()
    {
        return typeof(DiagnosticIdentifiers)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(fieldInfo => ResourceEntryNameSuffixes, (fieldInfo, diagnosticName) => $"{fieldInfo.Name}{diagnosticName}");
    }
}