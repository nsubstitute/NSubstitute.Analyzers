using Microsoft.CodeAnalysis.Text;

namespace NSubstitute.Analyzers.Tests.Shared.Text;

public readonly struct LinePositionSpanInfo
{
    public LinePositionSpanInfo(in LinePositionInfo start, in LinePositionInfo end)
    {
        Start = start;
        End = end;
    }

    public LinePositionInfo Start { get; }

    public LinePositionInfo End { get; }

    public TextSpan Span => TextSpan.FromBounds(Start.Index, End.Index);

    public LinePositionSpan LineSpan => new(Start.LinePosition, End.LinePosition);
}