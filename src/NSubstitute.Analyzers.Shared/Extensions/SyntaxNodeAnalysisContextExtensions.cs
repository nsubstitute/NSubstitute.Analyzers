using System.Threading;
using Microsoft.CodeAnalysis.Diagnostics;
using NSubstitute.Analyzers.Shared.Settings;

namespace NSubstitute.Analyzers.Shared.Extensions
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        internal static AnalyzersSettings GetSettings(this SyntaxNodeAnalysisContext context, CancellationToken cancellationToken)
        {
            return context.Options.GetSettings(cancellationToken);
        }
    }
}