using System.Linq;
using Microsoft.CodeAnalysis;

namespace NSubstitute.Analyzers.Tests.Shared.Extensions;

public static class DiagnosticDescriptorExtensions
{
    public static DiagnosticDescriptor OverrideMessage(this DiagnosticDescriptor diagnosticDescriptor, string overridenMessage)
    {
        return new DiagnosticDescriptor(
            diagnosticDescriptor.Id,
            diagnosticDescriptor.Title.ToString(),
            overridenMessage,
            diagnosticDescriptor.Category,
            diagnosticDescriptor.DefaultSeverity,
            diagnosticDescriptor.IsEnabledByDefault,
            diagnosticDescriptor.Description.ToString(),
            diagnosticDescriptor.HelpLinkUri,
            diagnosticDescriptor.CustomTags?.ToArray());
    }
}