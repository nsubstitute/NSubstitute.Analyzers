using NSubstitute.Analyzers.Shared;

namespace NSubstitute.Analyzers.CSharp
{
    internal class DiagnosticDescriptorsProvider : AbstractDiagnosticDescriptorsProvider<DiagnosticDescriptorsProvider>
    {
        public static DiagnosticDescriptorsProvider Instance { get; } = new DiagnosticDescriptorsProvider();

        private DiagnosticDescriptorsProvider()
        {
        }
    }
}