using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared.DocumentationTests;

public class DocumentationTests
{
    private static readonly ImmutableArray<DiagnosticDescriptor> DiagnosticDescriptors = DiagnosticIdentifierTests
        .DiagnosticIdentifierTests.DiagnosticDescriptors
        .DistinctBy(diag => diag.Id) // NS5000 is duplicated as it is used in two flavours extension/non-extension method usage
        .OrderBy(diag => diag.Id).ToImmutableArray();

    public static IEnumerable<object[]> DiagnosticDescriptorTestCases { get; } = DiagnosticDescriptors
        .Select(diag => new object[] { diag }).ToList();

    [Theory]
    [MemberData(nameof(DiagnosticDescriptorTestCases))]
    public void DiagnosticDocumentation_ShouldHave_ProperHeadings(DiagnosticDescriptor descriptor)
    {
        var markdownDocument = GetParsedDocumentation(descriptor);

        var layout = GetLayoutByHeadings(markdownDocument);

        markdownDocument.First().Should().BeOfType<HeadingBlock>();
        AssertHeadingsLayout(layout, descriptor.Id);
    }

    [Theory]
    [MemberData(nameof(DiagnosticDescriptorTestCases))]
    public void DiagnosticDocumentation_ShouldHave_ProperContent(DiagnosticDescriptor descriptor)
    {
        var markdownDocument = GetParsedDocumentation(descriptor);

        var layout = GetLayoutByHeadings(markdownDocument);

        markdownDocument.First().Should().BeOfType<HeadingBlock>();
        AssertContent(layout, descriptor.Id, descriptor.Category);
    }

    [Theory]
    [MemberData(nameof(DiagnosticDescriptorTestCases))]
    public void RulesSummary_ShouldHave_ContentCorrespondingToRuleFile(DiagnosticDescriptor descriptor)
    {
        var documentationDirectory = GetRulesDocumentationDirectoryPath();
        var rulesSummaryFileInfo = new FileInfo(Path.Combine(documentationDirectory, "README.md"));
        var parsedDocumentation = GetLayoutByHeadings(GetParsedDocumentation(rulesSummaryFileInfo));

        AssertRulesSummaryRow(descriptor, parsedDocumentation);
    }

    private void AssertRulesSummaryRow(DiagnosticDescriptor descriptor, IReadOnlyList<HeadingContainer> parsedDocumentation)
    {
        var ruleRowLocation = DiagnosticDescriptors.IndexOf(descriptor);
        var rulesTable = parsedDocumentation.Single(container => GetBlockText(container.Heading) == "Rules")
            .Children.OfType<Table>().Single();

        // skip header row
        var ruleRow = rulesTable.OfType<TableRow>().Skip(1).ElementAt(ruleRowLocation);
        var cells = ruleRow.OfType<TableCell>().ToList();
        AssertRuleSummaryIdCell(cells.First(), descriptor);
        AssertRuleSummaryCategoryCell(cells.ElementAt(1), descriptor);
        AssertRuleSummaryCauseCell(cells.ElementAt(2), descriptor);
    }

    private static IReadOnlyList<Block> GetParsedDocumentation(DiagnosticDescriptor descriptor)
    {
        var directoryName = GetRulesDocumentationDirectoryPath();
        var fileInfo = new FileInfo(Path.Combine(directoryName, $"{descriptor.Id}.md"));

        return GetParsedDocumentation(fileInfo);
    }

    private static IReadOnlyList<Block> GetParsedDocumentation(FileInfo fileInfo)
    {
        var markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UsePreciseSourceLocation()
            .Build();

        var markdownDocument = Markdown.Parse(File.ReadAllText(fileInfo.FullName), markdownPipeline)
            .ToList();
        return markdownDocument;
    }

    private static string GetRulesDocumentationDirectoryPath()
    {
        var locations = new[] { "..", "..", "..", "..", "..", "documentation", "rules" };
        var rulesPath = string.Join(Path.DirectorySeparatorChar, locations);
        return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, rulesPath));
    }

    private IReadOnlyList<HeadingContainer> GetLayoutByHeadings(IReadOnlyList<Block> blocks)
    {
        var blockedLayout = new List<HeadingContainer>();

        HeadingBlock? currentHeadingBlock = null;
        HeadingContainer? currentContainer = null;

        foreach (var currentBlock in blocks)
        {
            if (currentBlock is HeadingBlock headingBlock)
            {
                currentHeadingBlock = headingBlock;
                currentContainer = new HeadingContainer(currentHeadingBlock, new List<Block>());
                blockedLayout.Add(currentContainer);
            }

            if (currentHeadingBlock != null && currentBlock is not HeadingBlock)
            {
                currentContainer!.Children.Add(currentBlock);
            }
        }

        return blockedLayout;
    }

    private void AssertHeadingsLayout(IReadOnlyList<HeadingContainer> layout, string ruleId)
    {
        layout.Should().HaveCountGreaterOrEqualTo(5);
        AssertHeading(layout[0].Heading, 1, ruleId);
        AssertHeading(layout[1].Heading, 2, "Cause");
        AssertHeading(layout[2].Heading, 2, "Rule description");
        AssertHeading(layout[3].Heading, 2, "How to fix violations");
        AssertHeading(layout[4].Heading, 2, "How to suppress violations");
    }

    private void AssertHeading(HeadingBlock heading, int expectedLevel, string expectedText)
    {
        var headingText = GetBlockText(heading);

        heading.Level.Should().Be(expectedLevel);
        headingText.Should().Be(expectedText);
    }

    private void AssertContent(IReadOnlyList<HeadingContainer> layout, string ruleId, string ruleCategory)
    {
        AssertTableContent(layout, ruleId, ruleCategory);
        AssertHeaderContentNonEmpty(layout);
    }

    private void AssertHeaderContentNonEmpty(IReadOnlyList<HeadingContainer> layout)
    {
        layout.Should().OnlyContain(container => container.Children != null && container.Children.Count > 0);
    }

    private void AssertTableContent(IReadOnlyList<HeadingContainer> layout, string ruleId, string ruleCategory)
    {
        var children = layout[0].Children;
        children.Should().HaveCount(1);
        children.Single().Should().BeOfType<HtmlBlock>();
        var expectedInfo = $@"<table>
<tr>
  <td>CheckId</td>
  <td>{ruleId}</td>
</tr>
<tr>
  <td>Category</td>
  <td>{ruleCategory}</td>
</tr>
</table>".Replace("\r", string.Empty); // rendering issue of markdig
        children.Single().As<HtmlBlock>().Lines.ToString().Should().Be(expectedInfo);
    }

    private void AssertRuleSummaryIdCell(TableCell cell, DiagnosticDescriptor descriptor)
    {
        var descendants = cell.Descendants().ToList();
        var linkInline = descendants.OfType<LinkInline>().Single();
        linkInline.Url.Should().Be($"{descriptor.Id}.md");
        linkInline.FirstChild.ToString().Should().Be(descriptor.Id);
    }

    private void AssertRuleSummaryCategoryCell(TableCell cell, DiagnosticDescriptor descriptor)
    {
        cell.OfType<ParagraphBlock>().Single().Inline.Single().ToString().Should().Be(descriptor.Category);
    }

    private void AssertRuleSummaryCauseCell(TableCell cell, DiagnosticDescriptor descriptor)
    {
        var ruleDocument = GetParsedDocumentation(descriptor);
        var layoutByHeadings = GetLayoutByHeadings(ruleDocument);
        var headingContainer = layoutByHeadings.Single(heading => GetBlockText(heading.Heading) == "Cause");

        var cellContent = GetBlockText(cell.OfType<ParagraphBlock>().Single());
        var ruleContent = GetBlockText(headingContainer.Children.OfType<ParagraphBlock>().Single());

        cellContent.Should().Be(ruleContent);
    }

    private static string GetBlockText(LeafBlock heading)
    {
        using var stringWriter = new StringWriter();
        var renderer = new NormalizeRenderer(stringWriter);
        renderer.Write(heading.Inline);
        return stringWriter.ToString();
    }

    private class HeadingContainer
    {
        public HeadingBlock Heading { get; }

        public List<Block> Children { get; }

        public HeadingContainer(HeadingBlock heading, List<Block> children)
        {
            Heading = heading;
            Children = children;
        }
    }
}