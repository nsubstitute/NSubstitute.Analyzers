using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

internal class CallInfoCallFinder : AbstractCallInfoFinder
{
    public static CallInfoCallFinder Instance { get; } = new ();

    private CallInfoCallFinder()
    {
    }
}