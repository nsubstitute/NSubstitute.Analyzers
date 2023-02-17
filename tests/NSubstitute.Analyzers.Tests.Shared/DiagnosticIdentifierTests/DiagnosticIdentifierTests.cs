using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Shared;
using NSubstitute.Analyzers.Shared.Extensions;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticIdentifierTests;

public class DiagnosticIdentifierTests
{
    private const string IdentifierPrefix = "NS";

    private static readonly FieldInfo[] DiagnosticIdentifierFields = typeof(DiagnosticIdentifiers)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fieldInfo => fieldInfo.IsLiteral && fieldInfo.IsInitOnly == false)
        .ToArray();

    private static readonly PropertyInfo[] DiagnosticDescriptorsProperties = typeof(DiagnosticDescriptors<SharedResourceManager>)
        .GetProperties(BindingFlags.Public | BindingFlags.Static)
        .ToArray();

    public static List<DiagnosticDescriptor> DiagnosticDescriptors { get; } = DiagnosticDescriptorsProperties.Select(desc => (DiagnosticDescriptor)desc.GetValue(null)!).ToList();

    [Fact]
    public void DiagnosticIdentifiers_ShouldHaveConstantValue()
    {
        DiagnosticIdentifierFields.Should().BeEquivalentTo(typeof(DiagnosticIdentifiers).GetFields());
    }

    [Fact]
    public void DiagnosticIdentifiers_StartsWithProperPrefix()
    {
        var invalidDiagnosticNames = DiagnosticIdentifierFields.Where(info => ((string)info.GetRawConstantValue()!).StartsWith(IdentifierPrefix) == false);

        invalidDiagnosticNames.Should().BeEmpty($"because all diagnostics should start with {IdentifierPrefix} prefix");
    }

    [Fact]
    public void DiagnosticIdentifiers_ShouldBelongToSpecificCategory()
    {
        var invalidCategories = DiagnosticIdentifierFields.Where(info => GetCategoryId(info) == 0);

        invalidCategories.Should().BeEmpty("because all diagnostics should belong to specific category");
    }

    [Fact]
    public void DiagnosticIdentifiers_Categories_ShouldHaveConsecutiveNumbers()
    {
        var groupedCategories = DiagnosticIdentifierFields.Select(GetCategoryId)
            .GroupBy(category => category)
            .Select(group => group.Key)
            .OrderBy(category => category)
            .ToList();

        var expectedCategories = Enumerable.Range(1, groupedCategories.Count);

        groupedCategories.Should().BeEquivalentTo(expectedCategories, "because category numbers should be consecutive");
    }

    [Fact]
    public void DiagnosticIdentifiers_WithinCategory_ShouldHaveConsecutiveNumbers()
    {
        var groupedIdentifiers = DiagnosticIdentifierFields
            .GroupBy(GetCategoryId)
            .Select(group => group.Select(GetDiagnosticId).OrderBy(diagnostic => diagnostic).ToList())
            .ToList();

        var expectedGroupedIdentifiers = groupedIdentifiers.Select(group => Enumerable.Range(0, group.Count));

        groupedIdentifiers.Should().BeEquivalentTo(expectedGroupedIdentifiers);
    }

    [Fact]
    public void DiagnosticDescriptors_Categories_ShouldMatchDiagnosticIdentifiers()
    {
        var descriptionEnumMap = ((DiagnosticCategory[])Enum.GetValues(typeof(DiagnosticCategory))).ToDictionary(value => value.GetDisplayName());

        var invalidCategoriesDescriptor = DiagnosticDescriptors.Where(desc => (int)descriptionEnumMap[desc.Category] != GetCategoryId(desc.Id));

        invalidCategoriesDescriptor.Should().BeEmpty("because descriptor category should match identifier category");
    }

    [Fact]
    public void DiagnosticDescriptors_HelpLinkUri_ShouldPointToProperDiagnosticDocumentation()
    {
        var invalidHelpLinkDescriptors = DiagnosticDescriptors
            .Where(diagnostic => diagnostic.HelpLinkUri != $"https://github.com/nsubstitute/NSubstitute.Analyzers/blob/master/documentation/rules/{diagnostic.Id}.md");

        invalidHelpLinkDescriptors.Should().BeEmpty();
    }

    private int GetCategoryId(FieldInfo fieldInfo)
    {
        return GetCategoryId((string)fieldInfo.GetRawConstantValue()!);
    }

    private int GetCategoryId(string diagnosticId)
    {
        var substringIndex = diagnosticId.IndexOf(IdentifierPrefix) + IdentifierPrefix.Length;
        return int.Parse(diagnosticId.Substring(substringIndex, 1));
    }

    private int GetDiagnosticId(FieldInfo fieldInfo)
    {
        return GetDiagnosticId((string)fieldInfo.GetRawConstantValue()!);
    }

    private int GetDiagnosticId(string diagnosticId)
    {
        var substringIndex = diagnosticId.IndexOf(IdentifierPrefix) + IdentifierPrefix.Length + 1;
        return int.Parse(diagnosticId.Substring(substringIndex));
    }
}