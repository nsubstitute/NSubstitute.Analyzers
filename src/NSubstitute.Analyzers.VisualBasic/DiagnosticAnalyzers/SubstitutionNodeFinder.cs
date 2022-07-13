using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.VisualBasic.DiagnosticAnalyzers;

internal sealed class SubstitutionNodeFinder : AbstractSubstitutionNodeFinder
{
    public static SubstitutionNodeFinder Instance { get; } = new ();

    private SubstitutionNodeFinder()
    {
    }
}