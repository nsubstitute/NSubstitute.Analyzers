using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.CodeAnalysis;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared.DocumentationTests
{
    public class DocumentationTests
    {
        public static IEnumerable<object[]> DiagnosticDescriptors { get; } = DiagnosticIdentifierTests.DiagnosticIdentifierTests.DiagnosticDescriptors.Select(diag => new object[] { diag }).ToList();

        [Theory]
        [MemberData(nameof(DiagnosticDescriptors))]
        public void DiagnosticDocumentation_ShouldHave_ProperHeadings(DiagnosticDescriptor descriptor)
        {
            var markdownDocument = GetParsedDocumentation(descriptor);

            var layout = GetLayoutByHeadings(markdownDocument);

            markdownDocument.First().Should().BeOfType<HeadingBlock>();
            AssertHeadingsLayout(layout, descriptor.Id);
        }

        [Theory]
        [MemberData(nameof(DiagnosticDescriptors))]
        public void DiagnosticDocumentation_ShouldHave_ProperContent(DiagnosticDescriptor descriptor)
        {
            var markdownDocument = GetParsedDocumentation(descriptor);

            var layout = GetLayoutByHeadings(markdownDocument);

            markdownDocument.First().Should().BeOfType<HeadingBlock>();
            AssertContent(layout, descriptor.Id, descriptor.Category);
        }

        private static List<Block> GetParsedDocumentation(DiagnosticDescriptor descriptor)
        {
            var directoryName = GetRulesDocumentationDirectoryPath();
            var fileInfo = new FileInfo(Path.Combine(directoryName, $"{descriptor.Id}.md"));

            return GetParsedDocumentation(fileInfo);
        }

        private static List<Block> GetParsedDocumentation(FileInfo fileInfo)
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
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), rulesPath));
        }

        private List<HeadingContainer> GetLayoutByHeadings(List<Block> blocks)
        {
            var blockedLayout = new List<HeadingContainer>();

            HeadingBlock currentHeadingBlock = null;
            HeadingContainer currentContainer = null;

            foreach (var currentBlock in blocks)
            {
                if (currentBlock is HeadingBlock headingBlock)
                {
                    currentHeadingBlock = headingBlock;
                    currentContainer = new HeadingContainer(currentHeadingBlock, new List<Block>());
                    blockedLayout.Add(currentContainer);
                }

                if (currentHeadingBlock != null && !(currentBlock is HeadingBlock))
                {
                    currentContainer.Children.Add(currentBlock);
                }
            }

            return blockedLayout;
        }

        private void AssertHeadingsLayout(List<HeadingContainer> layout, string ruleId)
        {
            layout.Should().HaveCount(5);
            AssertHeading(layout[0].Heading, 1, ruleId);
            AssertHeading(layout[1].Heading, 2, "Cause");
            AssertHeading(layout[2].Heading, 2, "Rule description");
            AssertHeading(layout[3].Heading, 2, "How to fix violations");
            AssertHeading(layout[4].Heading, 2, "How to suppress violations");
        }

        private void AssertHeading(HeadingBlock heading, int expectedLevel, string expectedText)
        {
            var inline = heading.Inline.ToList();

            inline.Should().HaveCount(1);
            heading.Level.Should().Be(expectedLevel);
            inline[0].ToString().Should().Be(expectedText);
        }

        private void AssertContent(List<HeadingContainer> layout, string ruleId, string ruleCategory)
        {
            AssertTableContent(layout, ruleId, ruleCategory);
            AssertHeaderContentNonEmpty(layout);
        }

        private void AssertHeaderContentNonEmpty(List<HeadingContainer> layout)
        {
            layout.Should().OnlyContain(container => container.Children != null && container.Children.Any());
        }

        private void AssertTableContent(List<HeadingContainer> layout, string ruleId, string ruleCategory)
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

        private static IEnumerable<T> Traverse<T>(
            IEnumerable<T> items,
            Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in childSelector(next))
                {
                    stack.Push(child);
                }
            }
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
}