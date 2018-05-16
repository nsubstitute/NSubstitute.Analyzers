#load "./parameters.cake"
#load "./version.cake"
#load "./paths.cake"

// Install tools.
#tool "nuget:https://www.nuget.org/api/v2?package=gitreleasemanager"
#tool "nuget:https://www.nuget.org/api/v2?package=GitVersion.CommandLine"
#tool "nuget:https://www.nuget.org/api/v2?package=coveralls.io"
#tool "nuget:https://www.nuget.org/api/v2?package=OpenCover"
#tool "nuget:https://www.nuget.org/api/v2?package=ReportGenerator"
#addin "nuget:https://www.nuget.org/api/v2?package=cake.coveralls"

var parameters = BuildParameters.GetParameters(Context);
var buildVersion = BuildVersion.Calculate(Context);
var paths = BuildPaths.GetPaths(Context, parameters, buildVersion);
var publishingError = false;
var packages = BuildPackages.GetPackages(paths, buildVersion);
var releaseNotes = ParseReleaseNotes(paths.Files.AllReleaseNotes);


Setup(context =>
{
    Information("Building version {0} of NSubstitute.Analyzers", buildVersion.SemVersion);
    if (!DirectoryExists(paths.Directories.Artifacts))
    {
        CreateDirectory(paths.Directories.Artifacts);
    }

    if (!DirectoryExists(paths.Directories.TestResults))
    {
        CreateDirectory(paths.Directories.TestResults);
    }

    if (FileExists(paths.Files.CurrentReleaseNotes))
    {
        DeleteFile(paths.Files.CurrentReleaseNotes);
    }
});

Task("Clean")
    .Does(() =>
{
    DotNetCoreClean(paths.Files.Solution.ToString());
    CleanDirectories(paths.Directories.ToClean);    
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .IsDependentOn("Patch-AssemblyInfo")
    .Does(() =>
{
    DotNetCoreRestore(paths.Files.Solution.ToString());
});

Task("Run-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    Action<ICakeContext> testAction = context =>
    {
        context.DotNetCoreVSTest(paths.Files.TestAssemblies, new DotNetCoreVSTestSettings
        {
            Framework = "FrameworkCore10",
            Parallel = true,
            Platform = VSTestPlatform.x64
        });
    };

    if (parameters.SkipOpenCover)
    {
        testAction(Context);
    }
    else
    {
        OpenCover(testAction,
                        paths.Files.TestCoverageOutput,
                        new OpenCoverSettings
                        {
                            ReturnTargetCodeOffset = 0,
                            OldStyle = true,
                            ArgumentCustomization = args => args.Append("-mergeoutput"),


                        }
                        .WithFilter("+[NSubstitute.Analyzers*]* -[NSubstitute.Analyzers.Test*]*")
                        .ExcludeByAttribute("*.ExcludeFromCodeCoverage*")
                        .ExcludeByFile("*/*Designer.cs;*/*.g.cs;*/*.g.i.cs"));
        ReportGenerator(paths.Files.TestCoverageOutput, paths.Directories.TestResults);
    }

});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild(paths.Files.Solution.ToString(), new DotNetCoreBuildSettings
    {
        Configuration = parameters.Configuration,
        NoRestore = true
    });
});

Task("NuGet-Pack")
.IsDependentOn("Run-Tests")
.WithCriteria(val => parameters.Configuration == "Release")
.Does(() =>
{
    foreach(var projectFile in paths.Files.ProjectsToPack)
    {
        var settings = new DotNetCorePackSettings
        {
            NoBuild = true,
            VersionSuffix = buildVersion.SemVersion.ToString(),
            Configuration = parameters.Configuration,
            OutputDirectory = paths.Directories.Artifacts
        };

        DotNetCorePack(projectFile.ToString(), settings);
    }
});

Task("Publish")
    .IsDependentOn("NuGet-Pack")
    .WithCriteria(context => parameters.ShouldPublish && releaseNotes.Version.ToString() == buildVersion.SemVersion)
    .Does(() =>
{
    var apiKey = EnvironmentVariable("NUGET_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    var apiUrl = EnvironmentVariable("NUGET_API_URL");
    if (string.IsNullOrEmpty(apiUrl))
    {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    NuGetPush(packages.NuGetPackage, new NuGetPushSettings
    {
        ApiKey = apiKey,
        Source = apiUrl
    });
}).OnError(exception =>
{
    Information("Publish Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Patch-AssemblyInfo")
.WithCriteria(() => !parameters.IsLocalBuild)
.Does(() =>
{
    GitVersion(new GitVersionSettings
    {
        UpdateAssemblyInfo = true,
        WorkingDirectory = paths.Directories.RootDir
    });
});


Task("Upload-Coverage-Report")
    .WithCriteria(() => FileExists(paths.Files.TestCoverageOutput))
    .WithCriteria(() => !parameters.IsLocalBuild)
    .WithCriteria(() => !parameters.IsPullRequest)
    .WithCriteria(() => parameters.IsMaster)
    .IsDependentOn("Publish")
    .Does(() =>
{
    var repoKey = EnvironmentVariable("COVERALLS_REPO_TOKEN");
    if (string.IsNullOrEmpty(repoKey))
    {
        throw new InvalidOperationException("Could not resolve Coveralls Repo key.");
    }

    CoverallsIo(paths.Files.TestCoverageOutput, new CoverallsIoSettings
    {
        RepoToken = repoKey
    });
});

Task("AppVeyor")
  .IsDependentOn("Upload-Coverage-Report")
  .IsDependentOn("Publish")
  .Finally(() =>
{
    if (publishingError)
    {
        throw new Exception("An error occurred during the publishing of Cake.  All publishing tasks have been attempted.");
    }
});

Teardown(context =>
{
    var result = context.Successful ? "succeeded" : "failed";
    Information("NSubstitute.Analyzers {0} - build {1}", buildVersion.SemVersion, result);
});

RunTarget(parameters.Target);