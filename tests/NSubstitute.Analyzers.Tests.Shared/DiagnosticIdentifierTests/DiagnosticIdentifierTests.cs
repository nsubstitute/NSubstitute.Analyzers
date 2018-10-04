using System.Linq;
using System.Reflection;
using FluentAssertions;
using NSubstitute.Analyzers.Shared;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared.DiagnosticIdentifierTests
{
    public class DiagnosticIdentifierTests
    {
        private const string IdentifierPrefix = "NS";

        private readonly FieldInfo[] _diagnosticIdentifiers = typeof(DiagnosticIdentifiers)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fieldInfo => fieldInfo.IsLiteral && fieldInfo.IsInitOnly == false)
            .ToArray();

        [Fact]
        public void DiagnosticIdentifiers_HasConstantValue()
        {
            _diagnosticIdentifiers.Should().BeEquivalentTo(typeof(DiagnosticIdentifiers).GetFields());
        }

        [Fact]
        public void DiagnosticIdentifiers_StartsWithProperPrefix()
        {
            var invalidDiagnosticNames = _diagnosticIdentifiers.Where(info => ((string)info.GetRawConstantValue()).StartsWith(IdentifierPrefix) == false);

            invalidDiagnosticNames.Should().BeEmpty($"because all diagnostics should start with {IdentifierPrefix} prefix");
        }

        [Fact]
        public void DiagnosticIdentifiers_BelongToSpecificCategory()
        {
            var invalidCategories = _diagnosticIdentifiers.Where(info => GetCategoryId(info) == 0);

            invalidCategories.Should().BeEmpty("because all diagnostics should belong to specific category");
        }

        [Fact]
        public void DiagnosticIdentifiersCategories_ShouldHaveConsecutiveNumbers()
        {
            var groupedCategories = _diagnosticIdentifiers.Select(GetCategoryId)
                .GroupBy(category => category)
                .Select(group => group.Key)
                .OrderBy(category => category)
                .ToList();

            var expectedCategories = Enumerable.Range(1, groupedCategories.Count);

            groupedCategories.Should().BeEquivalentTo(expectedCategories, "because category numbers should be consecutive");
        }

        [Fact]
        public void DiagnosticIdentifiersWithinCategory_ShouldHaveConsecutiveNumbers()
        {
            var groupedIdentifiers = _diagnosticIdentifiers
                .GroupBy(GetCategoryId)
                .Select(group => group.Select(GetDiagnosticId).OrderBy(diagnostic => diagnostic).ToList())
                .ToList();

            var expectedGroupedIdentifiers = groupedIdentifiers.Select(group => Enumerable.Range(1, group.Count));

            groupedIdentifiers.Should().BeEquivalentTo(expectedGroupedIdentifiers);
        }

        private int GetCategoryId(FieldInfo fieldInfo)
        {
            return GetCategoryId((string)fieldInfo.GetRawConstantValue());
        }

        private int GetCategoryId(string diagnosticId)
        {
            var substringIndex = diagnosticId.IndexOf(IdentifierPrefix) + IdentifierPrefix.Length;
            return int.Parse(diagnosticId.Substring(substringIndex, 1));
        }

        private int GetDiagnosticId(FieldInfo fieldInfo)
        {
            return GetDiagnosticId((string)fieldInfo.GetRawConstantValue());
        }

        private int GetDiagnosticId(string diagnosticId)
        {
            var substringIndex = diagnosticId.IndexOf(IdentifierPrefix) + IdentifierPrefix.Length + 1;
            return int.Parse(diagnosticId.Substring(substringIndex));
        }
    }
}