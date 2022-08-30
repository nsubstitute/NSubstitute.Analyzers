using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using NSubstitute.Analyzers.Tests.Shared;

namespace NSubstitute.Analyzers.Tests.VisualBasic;

public class VisualBasicWorkspaceFactory : WorkspaceFactory
{
    public static VisualBasicWorkspaceFactory Default { get; } = new(VisualBasicProjectOptions.Default);

    protected override string DocumentExtension { get; } = "vb";

    protected override string Language { get; } = LanguageNames.VisualBasic;

    protected override ProjectOptions ProjectOptions { get; }

    private VisualBasicProjectOptions VisualBasicProjectOptions => (VisualBasicProjectOptions)ProjectOptions;

    private VisualBasicWorkspaceFactory(ProjectOptions projectOptions)
    {
        ProjectOptions = projectOptions;
    }

    public VisualBasicWorkspaceFactory WithOptionStrictOn()
    {
        return WithOptionStrict(OptionStrict.On);
    }

    public VisualBasicWorkspaceFactory WithOptionStrict(OptionStrict optionStrict)
    {
        return new VisualBasicWorkspaceFactory(VisualBasicProjectOptions.WithOptionStrict(optionStrict));
    }

    public VisualBasicWorkspaceFactory WithAdditionalMetadataReferences(MetadataReference[] metadataReferences)
    {
        return new VisualBasicWorkspaceFactory(VisualBasicProjectOptions.WithAdditionalMetadataReferences(metadataReferences));
    }
}