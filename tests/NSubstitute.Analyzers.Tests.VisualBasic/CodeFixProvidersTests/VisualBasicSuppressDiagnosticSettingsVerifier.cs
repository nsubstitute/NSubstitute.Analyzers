using Microsoft.CodeAnalysis;
using Microsoft.VisualBasic.CompilerServices;
using NSubstitute.Analyzers.Tests.Shared.CodeFixProviders;

namespace NSubstitute.Analyzers.Tests.VisualBasic.CodeFixProvidersTests
{
    public abstract class VisualBasicSuppressDiagnosticSettingsVerifier : SuppressDiagnosticSettingsVerifier
    {
        private static readonly MetadataReference[] AdditionalReferences =
        {
            MetadataReference.CreateFromFile(typeof(StandardModuleAttribute).Assembly.Location)
        };

        protected VisualBasicSuppressDiagnosticSettingsVerifier()
            : base(VisualBasicWorkspaceFactory.Default
                .WithOptionStrictOn()
                .WithAdditionalMetadataReferences(AdditionalReferences))
        {
        }
    }
}