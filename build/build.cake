#load "./parameters.cake"
#load "./version.cake"
#load "./paths.cake"
#load "./releasenotes.cake"
#load "./table-of-contents.cake"

// Install modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.1.0

// Install tools.
#tool "dotnet:https://api.nuget.org/v3/index.json?package=GitVersion.Tool&version=5.8.1"
#tool "dotnet:https://api.nuget.org/v3/index.json?package=coveralls.net&version=1.0.0"
#tool "nuget:https://www.nuget.org/api/v2?package=ReportGenerator&version=5.0.2"
#addin "nuget:https://www.nuget.org/api/v2?package=Cake.Incubator&version=4.0.1"
#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"
#addin "nuget:https://www.nuget.org/api/v2?package=semver.core&version=2.0.0"

using Cake.Incubator.LoggingExtensions;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;

var parameters = BuildParameters.GetParameters(Context);
var buildVersion = BuildVersion.Calculate(Context);
var paths = BuildPaths.GetPaths(Context, parameters, buildVersion);
var packages = BuildPackages.GetPackages(paths, buildVersion);
var releaseNotes = ReleaseNotes.ParseAllReleaseNotes(Context, paths);

Setup(context =>
{
    Information("Building version {0} of NSubstitute.Analyzers with following parameters {1} {2}", buildVersion.SemVersion, Environment.NewLine, parameters.Dump());

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
    
    var testSettings = new DotNetCoreTestSettings
    {
        Framework = parameters.TargetFramework,
        NoBuild = true,
        NoRestore = true,
        Configuration = parameters.Configuration
    };

    if(parameters.SkipCodeCoverage == false)
    {
        testSettings.ArgumentCustomization = arg => arg.AppendSwitch("/p:CollectCoverage","=","True")
                                                       .AppendSwitch("/p:CoverletOutputFormat", "=", @"\""json,opencover\""")
                                                       .AppendSwitch("/p:MergeWith", "=", $@"""{paths.Files.TestCoverageOutputWithoutExtension.ToString()}.json""")
                                                       .AppendSwitch("/p:CoverletOutput", "=", $@"""{paths.Files.TestCoverageOutputWithoutExtension.ToString()}""")
                                                       .AppendSwitch("/p:ExcludeByAttribute", "=", @"\""GeneratedCodeAttribute,ExcludeFromCodeCoverage\""")
                                                       .AppendSwitch("/p:Exclude", "=", @"\""[xunit.*]*,[NSubstitute.Analyzers.Test*]*,[NSubstitute.Analyzers.Test*]*,[NSubstitute.Analyzers.Benchmark*]*\""")
                                                       .AppendSwitch("/p:Include", "=", "[NSubstitute.Analyzers*]*")
                                                       .Append("-- RunConfiguration.NoAutoReporters=true");
    }

    DotNetCoreTest(paths.Files.Solution.MakeAbsolute(Context.Environment).ToString(), testSettings);

    if(parameters.SkipCodeCoverage == false)
    {
        var reportGeneratorWorkingDir = Context.Environment.WorkingDirectory
                                                       .Combine("tools")
                                                       .Combine("ReportGenerator.5.0.2")
                                                       .Combine("tools")
                                                       .Combine("net6.0");

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

Task("Wait-For-Pending-Jobs")
    .WithCriteria(context => parameters.ShouldPublish)
    .Does(async () =>
{
    var apiToken = EnvironmentVariable("APPVEYOR_API_TOKEN");
    var buildVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION");
    var jobId = EnvironmentVariable("APPVEYOR_JOB_ID");

    if(string.IsNullOrEmpty(apiToken))
        throw new InvalidOperationException("Could not resolve APPVEYOR_API_TOKEN.");

    if(string.IsNullOrEmpty(buildVersion))
        throw new InvalidOperationException("Could not resolve APPVEYOR_BUILD_VERSION.");

    if(string.IsNullOrEmpty(jobId))
        throw new InvalidOperationException("Could not resolve APPVEYOR_JOB_ID.");

    var ct = new CancellationTokenSource(TimeSpan.FromMinutes(15)).Token;
    var url = $"https://ci.appveyor.com/api/projects/nsubstitute/nsubstitute-analyzers/build/{buildVersion}";

    while(true)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders
                .Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            var httpResponseMessage = await client.GetAsync(url);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadAsStringAsync();

            var parsed = JToken.Parse(response);

            var nonSuccessfulJobs = parsed["build"].Value<JObject>()["jobs"].Value<JArray>()
                        .Cast<JObject>()
                        .Where(jObject => jObject["jobId"].Value<string>() != jobId && jObject["status"].Value<string>() != "success")
                        .ToList();

            if (nonSuccessfulJobs.Any() == false)
            {
                Information("All other build jobs in matrix finished");
                return;
            }

            var failedJobs = nonSuccessfulJobs.Where(jObject => jObject["status"].Value<string>() == "failed").ToList();

            if (failedJobs.Any())
            {
                var jobs = string.Join(",", failedJobs.Select(jObject => jObject["name"]));
                throw new CakeException($"Other build jobs failed, {jobs}");
            }

            var pendingJobs = nonSuccessfulJobs.Except(failedJobs).ToList();
            var pendingJobsNames = string.Join(",", pendingJobs.Select(jObject => jObject["name"]));
            
            Information("There are unfinished build jobs {0}, waiting for them to finish", pendingJobsNames);

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(30), ct);                             
        }
    }
});

Task("Publish")
    .IsDependentOn("NuGet-Pack")
    .IsDependentOn("Wait-For-Pending-Jobs")
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
});

Task("Upload-Coverage-Report")
    .WithCriteria(() => FileExists(paths.Files.TestCoverageOutput))
    .WithCriteria(() => parameters.UploadCoverageReport && !parameters.IsLocalBuild)
    .IsDependentOn("Publish")
    .Does(() =>
{
    var repoKey = EnvironmentVariable("COVERALLS_REPO_TOKEN");
    if (string.IsNullOrEmpty(repoKey))
    {
        throw new InvalidOperationException("Could not resolve coveralls repo key.");
    }
    var pathSegments = new [] { "tools",
                                ".store",
                                "coveralls.net",
                                "1.0.0",
                                "coveralls.net",
                                "1.0.0",
                                "tools",
                                "netcoreapp2.1",
                                "any" };

    var workingDir = pathSegments.Aggregate(Context.Environment.WorkingDirectory, (acc, seed) => acc.Combine(seed)); 

    var argumentBuilder = new ProcessArgumentBuilder()
        .Append("csmacnz.Coveralls.dll")
        .Append("--opencover")
        .AppendSwitch("-i"," ", paths.Files.TestCoverageOutput.MakeAbsolute(Context.Environment).ToString())
        .AppendSwitch("--repoToken"," ", repoKey);

        var exitCode = StartProcess("dotnet", new ProcessSettings
        {
            WorkingDirectory = workingDir,
            Arguments = argumentBuilder
        });

        if(exitCode != 0)
        {
            throw new CakeException($"Cannot upload coverage report, the process returned non-zero {exitCode} exit code");
        }
    
});

Task("AppVeyor")
  .IsDependentOn("Upload-Coverage-Report")
  .IsDependentOn("Publish");

Task("GenerateDocTableOfContents")
    .Does(() =>
{
    var rulesDir = paths.Directories.RootDir.Combine("documentation").Combine("rules");
    var header = @"
## Rules

| ID       | Category      | Cause |
|---|---|---|
";
    Information("Generating Table of Contents for {0}", rulesDir);
    var entries =
        GetFiles($"{rulesDir}/NS*.md")
            .Select(TableOfContentsEntry.Parse)
            .OrderBy(entry => entry.CheckId);
    var contents = header + string.Join("\n",
        entries.Select(entry => $"| [{entry.CheckId}]({entry.CheckId}.md) | {entry.Category} | {entry.Description} |")
    );

    var target = $"{rulesDir}/README.md";
    System.IO.File.WriteAllText(target, contents);
    Information("Generated Table of Context: {0}", target);
});

Teardown(context =>
{
    var result = context.Successful ? "succeeded" : "failed";
    Information("NSubstitute.Analyzers {0} - build {1}", buildVersion.SemVersion, result);
});

RunTarget(parameters.Target);