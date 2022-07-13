using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

internal class CallInfoCallFinder : AbstractCallInfoFinder
{
    public static CallInfoCallFinder Instance { get; } = new ();

    private CallInfoCallFinder()
    {
    }
}