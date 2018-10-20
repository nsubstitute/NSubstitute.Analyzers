using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Markdig;
using Markdig.Syntax;
using Xunit;

namespace NSubstitute.Analyzers.Tests.Shared.DocumentationTests
{
    public class DocumentationTests
    {
        [Fact]
        public void DiagnosticDocumentation_ShouldHave_ProperHeadings()
        {
            var documentation = @"# NS1001

<table>
<tr>
  <td>CheckId</td>
  <td>NS1001</td>
</tr>
<tr>
  <td>Category</td>
  <td>Non virtual substitution</td>
</tr>
</table>

## Cause

NSubstitute used with non-virtual members of class.

## Rule description

A violation of this rule occurs when NSubstitute's features like:
- `Received`
- `ReceivedWithAnyArgs`
- `DidNotReceive()`
- `DidNotReceiveWithAnyArgs()`

are used with non-virtual members of a class.

## How to fix violations

To fix a violation of this rule, make the member of your class virtual or substitute for interface.

## How to suppress violations

This warning can only be suppressed by disabling the warning in the **ruleset** file for the project.";
            var markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePreciseSourceLocation()
                .Build();

            var markdownDocument = Markdown.Parse(documentation, markdownPipeline)
                .ToList();

            var layout = GetLayoutByHeadings(markdownDocument);

            markdownDocument.First().Should().BeOfType<HeadingBlock>();
            AssertHeadingsLayout(layout, "NS1001");
        }

        [Fact]
        public void DiagnosticDocumentation_ShouldHave_ProperContent()
        {
            var documentation = @"# NS1001

<table>
<tr>
  <td>CheckId</td>
  <td>NS1001</td>
</tr>
<tr>
  <td>Category</td>
  <td>Non virtual substitution</td>
</tr>
</table>

## Cause

NSubstitute used with non-virtual members of class.

## Rule description

A violation of this rule occurs when NSubstitute's features like:
- `Received`
- `ReceivedWithAnyArgs`
- `DidNotReceive()`
- `DidNotReceiveWithAnyArgs()`

are used with non-virtual members of a class.

## How to fix violations

To fix a violation of this rule, make the member of your class virtual or substitute for interface.

## How to suppress violations

This warning can only be suppressed by disabling the warning in the **ruleset** file for the project.";

            var markdownPipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePreciseSourceLocation()
                .Build();

            var markdownDocument = Markdown.Parse(documentation, markdownPipeline)
                .ToList();

            var layout = GetLayoutByHeadings(markdownDocument);

            markdownDocument.First().Should().BeOfType<HeadingBlock>();
            AssertContent(layout, "NS1001", "Non virtual substitution");
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

        private void AssertContent(List<HeadingContainer> layout, string ruleId, string ruleCategory)
        {
            AssertTableContent(layout, ruleId, ruleCategory);
        }

        private void AssertTableContent(List<HeadingContainer> layout, string ruleId, string ruleCategory)
        {
            var children = layout[0].Children;
            children.Should().HaveCount(1);
            children.Single().Should().BeOfType<HtmlBlock>();
            var formattableString = $@"<table>
<tr>
  <td>CheckId</td>
  <td>{ruleId}</td>
</tr>
<tr>
  <td>Category</td>
  <td>{ruleCategory}</td>
</tr>
</table>".Replace("\r", string.Empty);
            children.Single().As<HtmlBlock>().Lines.ToString().Should().Be(formattableString);
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