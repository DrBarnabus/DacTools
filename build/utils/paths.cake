public class BuildPaths
{
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(ICakeContext context, BuildParameters parameters, string configuration, BuildVersion version)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (string.IsNullOrEmpty(configuration))
            throw new ArgumentNullException(nameof(configuration));

        if (version is null)
            throw new ArgumentNullException(nameof(version));

        var semVersion = version.SemVersion;

        var artifactsDir = (DirectoryPath)(context.Directory("./artifacts") + context.Directory("v" + semVersion));
        var artifactsBinDir = artifactsDir.Combine("bin");

        var buildArtifactDir = artifactsDir.Combine("build-artifact");
        var testResultsOutputDir = artifactsDir.Combine("test-results");

        // Directories
        var buildDirectories = new BuildDirectories(
            artifactsDir,
            artifactsBinDir,
            buildArtifactDir,
            testResultsOutputDir
        );

        return new BuildPaths
        {
            Directories = buildDirectories
        };
    }
}

public class BuildDirectories
{
    public DirectoryPath Artifacts { get; private set; }
    public DirectoryPath ArtifactsBin { get; private set; }
    public DirectoryPath BuildArtifact { get; private set; }
    public DirectoryPath TestResultsOutput { get; private set; }

    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath artifactsDir,
        DirectoryPath artifactsBinDir,
        DirectoryPath buildArtifactDir,
        DirectoryPath testResultsOutputDir)
    {
        Artifacts = artifactsDir;
        ArtifactsBin = artifactsBinDir;
        BuildArtifact = buildArtifactDir;
        TestResultsOutput = testResultsOutputDir;
        ToClean = new [] {
            Artifacts,
            ArtifactsBin,
            BuildArtifact,
            TestResultsOutput
        };
    }
}
