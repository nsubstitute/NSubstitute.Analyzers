public class BuildVersion
{
    public string Version { get; private set; }
    public string SemVersion { get; private set; }

    public static BuildVersion Calculate(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var version = context.GitVersion(new GitVersionSettings
            {
                OutputType = GitVersionOutput.Json,
            });
            
        return new BuildVersion
        {
            Version = version.MajorMinorPatch,
            SemVersion = version.LegacySemVer
        };
    }
}