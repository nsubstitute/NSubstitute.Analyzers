using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.VisualBasic.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.VisualBasic)]
    internal class DefaultSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
    {
    }
}