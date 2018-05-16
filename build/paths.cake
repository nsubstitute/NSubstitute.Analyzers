#load "./parameters.cake"
#load "./version.cake"
using System.Collections.Generic;
using System.Linq;

public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(ICakeContext context, BuildParameters parameters, BuildVersion version)
    {
        var configuration =  parameters.Configuration;
        var buildDirectories = GetBuildDirectories(context, version);
        var testAssemblies = buildDirectories.TestDirs
                                             .Select(dir => dir.Combine("bin")
                                                               .Combine(configuration)
                                                               .Combine(parameters.TargetFramework)
                                                               .CombineWithFilePath(dir.GetDirectoryName() + ".dll"))
                                             .ToList();
        var projectsToPack = new List<FilePath>
        {
            buildDirectories.SrcRootDir.CombineWithFilePath("NSubstitute.Analyzers/NSubstitute.Analyzers.CSharp.csproj"),
            buildDirectories.SrcRootDir.CombineWithFilePath("NSubstitute.Analyzers/NSubstitute.Analyzers.VisualBasic.csproj")
        };

        var buildFiles = new BuildFiles(
            buildDirectories.RootDir.CombineWithFilePath("NSubstitute.Analyzers.sln"),
            buildDirectories.TestResults.CombineWithFilePath("OpenCover.xml"),
            buildDirectories.RootDir.CombineWithFilePath("ReleaseNotes.md"),
            buildDirectories.Artifacts.CombineWithFilePath("ReleaseNotes.md"),
            buildDirectories.Artifacts.CombineWithFilePath("DupOutpuFinder.xml"),
            projectsToPack,
            testAssemblies);
        
        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }

    public static BuildDirectories GetBuildDirectories(ICakeContext context, BuildVersion version)
    {
        var rootDir = context.MakeAbsolute((DirectoryPath)context.Directory("../"));
        var artifacts = rootDir.Combine(".artifacts");
        var testResults = artifacts.Combine("Test-Results");
        
        var csharpAnalyzerTestDir = rootDir.Combine("tests").Combine("NSubstitute.Analyzers.Test.CSharp");
        
        var visualBasicAnalyzerTestDir = rootDir.Combine("tests").Combine("NSubstitute.Analyzers.Test.VisualBasic");
        
        var testDirs = new []{
                                csharpAnalyzerTestDir,
                                visualBasicAnalyzerTestDir
                            };
        var toClean = new[] {
                                 testResults
                            };
        return new BuildDirectories(rootDir,
                                    rootDir.Combine("src"),
                                    rootDir.Combine("tests"),
                                    artifacts,
                                    testResults,
                                    testDirs, 
                                    toClean);
    }
}

public class BuildFiles
{
    public FilePath Solution { get; private set; }
    public FilePath TestCoverageOutput { get; set;}
    public FilePath AllReleaseNotes { get; private set; }
    public FilePath CurrentReleaseNotes { get; private set; }
    public FilePath DupeFinderOutput { get; private set; }
    public ICollection<FilePath> ProjectsToPack { get; private set; }
    public ICollection<FilePath> TestAssemblies { get; private set; }

    public BuildFiles(FilePath solution,
                      FilePath testCoverageOutput,
                      FilePath allReleaseNotes,
                      FilePath currentReleaseNote,
                      FilePath dupeFinderOutput,
                      ICollection<FilePath> projectToPack,
                      ICollection<FilePath> testAssemblies)
    {
        Solution = solution;
        TestAssemblies = testAssemblies;
        TestCoverageOutput = testCoverageOutput;
        AllReleaseNotes = allReleaseNotes;
        CurrentReleaseNotes = currentReleaseNote;
        DupeFinderOutput = dupeFinderOutput;
        ProjectsToPack = projectToPack;
    }
}

public class BuildDirectories
{
    public DirectoryPath RootDir { get; private set; }

    public DirectoryPath SrcRootDir { get; private set; }
    public DirectoryPath TestsRootDir { get; private set; }
    public DirectoryPath Artifacts { get; private set; }
    public DirectoryPath TestResults { get; private set; }
    public ICollection<DirectoryPath> TestDirs { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath rootDir,
        DirectoryPath srcRootDir,
        DirectoryPath testsRootDir,
        DirectoryPath artifacts,
        DirectoryPath testResults,
        ICollection<DirectoryPath> testDirs,
        ICollection<DirectoryPath> toClean)
    {
        RootDir = rootDir;
        SrcRootDir = srcRootDir;
        TestsRootDir = testsRootDir;
        Artifacts = artifacts;
        TestDirs = testDirs;
        ToClean = toClean;
        TestResults = testResults;
    }
}

public class BuildPackages
{
    public FilePath NuGetPackage { get; private set;}
    public FilePath ZipPackage { get; private set; }
    public BuildPackages(FilePath nuGetPackage, FilePath zipPackage)
    {
        this.NuGetPackage = nuGetPackage;
        this.ZipPackage = zipPackage;
    }

    public static BuildPackages GetPackages(BuildPaths paths, BuildVersion version)
    {
        var fileNameWithoutExtension = "NSubstitute.Analyzers." + version.SemVersion;
        return new BuildPackages(
            paths.Directories.Artifacts.CombineWithFilePath(fileNameWithoutExtension + ".nupkg"),
            paths.Directories.Artifacts.CombineWithFilePath(fileNameWithoutExtension + ".zip"));
    }
}