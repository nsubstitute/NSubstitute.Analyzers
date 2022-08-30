using Microsoft.CodeAnalysis.Text;

namespace NSubstitute.Analyzers.Tests.Shared.Text;

public readonly struct LinePositionInfo
{
    public LinePositionInfo(int index, int lineIndex, int columnIndex)
    {
        Index = index;
        LineIndex = lineIndex;
        ColumnIndex = columnIndex;
    }

    public int Index { get; }

    public int LineIndex { get; }

    public int ColumnIndex { get; }

    public LinePosition LinePosition => new(LineIndex, ColumnIndex);
}