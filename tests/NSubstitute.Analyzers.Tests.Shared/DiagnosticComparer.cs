using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Tests.Shared;

public static class DiagnosticComparer
{
    public static IComparer<Diagnostic> Span { get; } = new DiagnosticSpanStartComparer();

    private class DiagnosticSpanStartComparer : IComparer<Diagnostic>
    {
        public int Compare(Diagnostic x, Diagnostic y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x == null)
            {
                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return x.Location.SourceSpan.Start.CompareTo(y.Location.SourceSpan.Start);
        }
    }
}