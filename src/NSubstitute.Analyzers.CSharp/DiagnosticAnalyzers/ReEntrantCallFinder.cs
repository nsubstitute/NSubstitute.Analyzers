using NSubstitute.Analyzers.Shared.DiagnosticAnalyzers;

namespace NSubstitute.Analyzers.CSharp.DiagnosticAnalyzers;

internal class ReEntrantCallFinder : AbstractReEntrantCallFinder
{
    public static ReEntrantCallFinder Instance { get; } = new (SubstitutionNodeFinder.Instance);

    private ReEntrantCallFinder(ISubstitutionNodeFinder substitutionNodeFinder)
        : base(substitutionNodeFinder)
    {
    }
}