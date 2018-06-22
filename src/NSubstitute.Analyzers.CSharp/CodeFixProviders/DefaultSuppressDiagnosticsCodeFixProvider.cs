using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NSubstitute.Analyzers.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public class DefaultSuppressDiagnosticsCodeFixProvider : AbstractSuppressDiagnosticsCodeFixProvider
    {
    }
}