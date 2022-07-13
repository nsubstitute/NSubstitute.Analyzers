using Microsoft.CodeAnalysis;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.CSharp;

public class CSharpWorkspaceFactory : WorkspaceFactory
{
    public static CSharpWorkspaceFactory Default { get; } = new (CSharpProjectOptions.Default);

    protected override string DocumentExtension { get; } = "cs";

    protected override string Language { get; } = LanguageNames.CSharp;

    protected override ProjectOptions ProjectOptions { get; }

    private CSharpProjectOptions CSharpProjectOptions => (CSharpProjectOptions)ProjectOptions;

    public CSharpWorkspaceFactory WithWarningLevel(int warningLevel)
    {
        return new CSharpWorkspaceFactory(CSharpProjectOptions.WithWarningLevel(warningLevel));
    }

    public CSharpWorkspaceFactory WithAdditionalMetadataReferences(MetadataReference[] metadataReferences)
    {
        return new CSharpWorkspaceFactory(CSharpProjectOptions.WithAdditionalMetadataReferences(metadataReferences));
    }

    private CSharpWorkspaceFactory(CSharpProjectOptions projectOptions)
    {
        ProjectOptions = projectOptions;
    }
}