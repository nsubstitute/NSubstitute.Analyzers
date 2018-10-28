public class ReleaseNotes
{
    // default Cake parser doesnt handle pre-release versions
    private static string versionPattern = @"^### (?<Version>(?<Major>0|[1-9]\d*)\.(?<Minor>0|[1-9]\d*)\.(?<Patch>0|[1-9]\d*)(?<PreReleaseTagWithSeparator>-(?<PreReleaseTag>((0|[1-9]\d*|\d*[A-Z-a-z-][\dA-Za-z-]*))(\.(0|[1-9]\d*|\d*[A-Za-z-][\dA-Za-z-]*))*))?)";

    public string SemVersion { get; }

    public IReadOnlyList<string> Notes { get; }

    private ReleaseNotes(string semVersion, IReadOnlyList<string> notes)
    {
        SemVersion = semVersion;
        Notes = notes;
    }

    private ReleaseNotes(Cake.Common.ReleaseNotes cakeReleaseNotes)
    {
        var match = Regex.Match(cakeReleaseNotes.RawVersionLine, versionPattern);
        var versionGroup = match.Groups["Version"];
        if(match.Success == false || versionGroup == null || versionGroup.Success == false)
        {
            throw new InvalidOperationException($"Unable to parse {cakeReleaseNotes.RawVersionLine} as semantic version");
        }

        SemVersion = versionGroup.Value;
        Notes = cakeReleaseNotes.Notes;
    }

    public static IList<ReleaseNotes> ParseAllReleaseNotes(ICakeContext context, BuildPaths paths)
    {
        var releaseNotes = context.ParseAllReleaseNotes(paths.Files.AllReleaseNotes);
        return releaseNotes.Select(note => new ReleaseNotes(note))
                           .OrderByDescending(note => note.SemVersion)
                           .ToList();
    }
}