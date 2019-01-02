using System.Collections.Immutable;

namespace NSubstitute.Analyzers.Tests.Shared.Text
{
    public readonly struct TextParserResult
    {
        public string Text { get; }

        public ImmutableArray<LinePositionSpanInfo> Spans { get; }

        public TextParserResult(string text, ImmutableArray<LinePositionSpanInfo> spans)
        {
            Text = text;
            Spans = spans;
        }
    }
}
