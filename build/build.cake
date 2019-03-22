#load "./parameters.cake"
#load "./version.cake"
#load "./paths.cake"
#load "./releasenotes.cake"

// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.1.0

// Install tools.
#tool "dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=4.0.1-beta1-58"
#tool "nuget:https://www.nuget.org/api/v2?package=coveralls.io&version=1.4.2"
#tool "nuget:https://www.nuget.org/api/v2?package=ReportGenerator&version=4.0.4"
#addin "nuget:https://www.nuget.org/api/v2?package=cake.coveralls&version=0.8.0"

using System.Text.RegularExpressions;

var parameters = BuildParameters.GetParameters(Context);
var buildVersion = BuildVersion.Calculate(Context);
var paths = BuildPaths.GetPaths(Context, parameters, buildVersion);
var publishingError = false;
var packages = BuildPackages.GetPackages(paths, buildVersion);
var releaseNotes = ReleaseNotes.ParseAllReleaseNotes(Context, paths);

Setup(context =>
{
    Information("Building version {0} of NSubstitute.Analyzers", buildVersion.SemVersion);

    if(DirectoryExists(paths.Directories.Artifacts))
    {
        CleanDirectories(paths.Directories.ToClean);
    }

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
    /*
    Information("Creating file {0}", paths.Files.TestCoverageMergeFile.MakeAbsolute(Context.Environment).ToString());
    System.IO.File.WriteAllText(paths.Files.TestCoverageMergeFile.MakeAbsolute(Context.Environment).ToString(), "{}");
    */

    string releaseNotesVersion = releaseNotes[0].SemVersion;
    if (parameters.ShouldPublish && buildVersion.SemVersion.Equals(releaseNotesVersion, StringComparison.Ordinal) == false)
    {
        throw new InvalidOperationException($"Release notes version {releaseNotesVersion} doesnt match build version {buildVersion.SemVersion}");
    }
});

Task("Clean")
    .Does(() =>
{
    DotNetCoreClean(paths.Files.Solution.ToString());
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(paths.Files.Solution.ToString());
});

Task("Run-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    
    if (parameters.SkipOpenCover)
    {
        DotNetCoreVSTest(paths.Files.TestAssemblies, new DotNetCoreVSTestSettings
        {
            Framework = "FrameworkCore10",
            Parallel = true,
            Platform = VSTestPlatform.x64
        });

        return;
    }

    DotNetCoreTest(paths.Files.Solution.MakeAbsolute(Context.Environment).ToString(), new DotNetCoreTestSettings
    {
        Framework = "netcoreapp2.0",
        NoBuild = true,
        NoRestore = true,
        Configuration = parameters.Configuration,
        ArgumentCustomization = arg => arg.AppendSwitch("/p:CollectCoverage","=","True")
                                                       .AppendSwitch("/p:CoverletOutputFormat", "=", @"\""json,opencover\""")
                                                       .AppendSwitch("/p:MergeWith", "=", $@"""{paths.Files.TestCoverageOutputWithoutExtension.ToString()}.json""")
                                                       .AppendSwitch("/p:CoverletOutput", "=", $@"""{paths.Files.TestCoverageOutputWithoutExtension.ToString()}""")
                                                       .AppendSwitch("/p:ExcludeByAttribute", "=", @"\""GeneratedCodeAttribute,ExcludeFromCodeCoverage\""")
                                                       .AppendSwitch("/p:Exclude", "=", @"\""[xunit.*]*,[NSubstitute.Analyzers.Test*]*\""")
                                                       .AppendSwitch("/p:Include", "=", "[NSubstitute.Analyzers*]*")
        });

    var reportGeneratorWorkingDir = Context.Environment.WorkingDirectory
                                                       .Combine("tools")
                                                       .Combine("ReportGenerator.4.0.4")
                                                       .Combine("tools")
                                                       .Combine("netcoreapp2.0");

    Information(reportGeneratorWorkingDir);

    var argumentBuilder = new ProcessArgumentBuilder()
    .Append("ReportGenerator.dll")
    .Append($@"""-reports:{paths.Files.TestCoverageOutput.MakeAbsolute(Context.Environment).ToString()}""")
    .Append($@"""-targetdir:{paths.Directories.TestResults.MakeAbsolute(Context.Environment).ToString()}");

    var exitCode = StartProcess("dotnet", new ProcessSettings
    {
        WorkingDirectory = reportGeneratorWorkingDir,
        Arguments = argumentBuilder
    });

    if(exitCode != 0)
    {
        throw new CakeException($"Report generator returned non-zero {exitCode} exit code");
    }
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreBuild(paths.Files.Solution.ToString(), new DotNetCoreBuildSettings
    {
        Configuration = parameters.Configuration,
        NoRestore = true,
        ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full")
                                          .AppendSwitch("/p:Version", "=", buildVersion.SemVersion.ToString()),
        VersionSuffix = buildVersion.SemVersion.ToString()
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
    .WithCriteria(context => parameters.ShouldPublish)
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

    NuGetPush(packages.AllPackages, new NuGetPushSettings
    {
        ApiKey = apiKey,
        Source = apiUrl
    });
}).OnError(exception =>
{
    Information("Publish Task failed, but continuing with next Task...");
    publishingError = true;
});


Task("Upload-Coverage-Report")
    .WithCriteria(() => FileExists(paths.Files.TestCoverageOutput))
    .WithCriteria(() => !parameters.IsLocalBuild)
    .IsDependentOn("Publish")
    .Does(() =>
{
    var repoKey = EnvironmentVariable("COVERALLS_REPO_TOKEN");
    if (string.IsNullOrEmpty(repoKey))
    {
        throw new InvalidOperationException("Could not resolve coveralls repo key.");
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