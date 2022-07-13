using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.VisualBasic;

internal class DiagnosticDescriptorsProvider : AbstractDiagnosticDescriptorsProvider<DiagnosticDescriptorsProvider>
{
    public static DiagnosticDescriptorsProvider Instance { get; } = new ();

    private DiagnosticDescriptorsProvider()
    {
    }
}