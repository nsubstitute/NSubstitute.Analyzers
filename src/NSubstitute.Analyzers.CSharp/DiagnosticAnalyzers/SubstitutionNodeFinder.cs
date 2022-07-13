using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

/// <summary>
/// Finds nodes which are considered to be a part of substitution call. For instance substitute.Bar().Returns(1) will return substitute.Bar().
/// </summary>
internal sealed class SubstitutionNodeFinder : AbstractSubstitutionNodeFinder
{
    public static SubstitutionNodeFinder Instance { get; } = new ();

    private SubstitutionNodeFinder()
    {
    }
}