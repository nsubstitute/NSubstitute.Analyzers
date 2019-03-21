public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }
    public bool SkipOpenCover { get; set; }
    public bool IsMaster { get; private set; }
    public bool IsDev { get; private set; }
    public bool IsLocalBuild { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPullRequest { get; private set; }
    public string TargetFramework { get; private set; }
    public string TargetFrameworkFull { get; private set; }
    public bool IsRunningOnWindows { get; private set; }

    public bool ShouldPublish
    {
        get
        {
            return !IsLocalBuild && IsMaster && IsTagged && !IsPullRequest;
        }
    }
    
    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        var buildSystem = context.BuildSystem();

        return new BuildParameters {
            Target = context.Argument("target", "Build"),
            Configuration = context.Argument("configuration", "Release"),
            SkipOpenCover = context.Argument<bool>("SkipOpenCover", false),
            IsLocalBuild = buildSystem.IsLocalBuild,
            IsMaster = StringComparer.OrdinalIgnoreCase.Equals("master", buildSystem.AppVeyor.Environment.Repository.Branch),
            IsDev = StringComparer.OrdinalIgnoreCase.Equals("dev", buildSystem.AppVeyor.Environment.Repository.Branch),
            IsTagged = IsBuildTagged(buildSystem),
            IsPullRequest = IsPullRequestBuild(buildSystem),
            TargetFramework = "netcoreapp2.0",
            TargetFrameworkFull = "netstandard1.1",
            IsRunningOnWindows = context.IsRunningOnWindows()
        };
    }

    private static bool IsBuildTagged(BuildSystem buildSystem)
    {
        return buildSystem.AppVeyor.Environment.Repository.Tag.IsTag
            && !string.IsNullOrWhiteSpace(buildSystem.AppVeyor.Environment.Repository.Tag.Name);
    }

    private static bool IsPullRequestBuild(BuildSystem buildSystem)
    {
        return buildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
    }
}
