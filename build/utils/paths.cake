#load "./parameters.cake"

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
        var artifactsBinCoreFx21Dir = artifactsBinDir.Combine(parameters.CoreFxVersion21);
        var artifactsBinCoreFx30Dir = artifactsBinDir.Combine(parameters.CoreFxVersion30);
        var artifactsBinFullFx472Dir = artifactsBinDir.Combine(parameters.FullFxVersion472);

        var nativeDir = artifactsDir.Combine("native");
        var buildArtifactDir = artifactsDir.Combine("build-artifact");
        var testResultsOutputDir = artifactsDir.Combine("test-results");

        // Directories
        var buildDirectories = new BuildDirectories(
            artifactsDir,
            artifactsBinDir,
            artifactsBinCoreFx21Dir,
            artifactsBinCoreFx30Dir,
            artifactsBinFullFx472Dir,
            nativeDir,
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
    public DirectoryPath ArtifactsBinCoreFx21 { get; private set; }
    public DirectoryPath ArtifactsBinCoreFx30 { get; private set; }
    public DirectoryPath ArtifactsBinFullFx472 { get; private set; }
    public DirectoryPath Native { get; private set; }
    public DirectoryPath BuildArtifact { get; private set; }
    public DirectoryPath TestResultsOutput { get; private set; }

    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath artifactsDir,
        DirectoryPath artifactsBinDir,
        DirectoryPath artifactsBinCoreFx21Dir,
        DirectoryPath artifactsBinCoreFx30Dir,
        DirectoryPath artifactsBinFullFx472Dir,
        DirectoryPath nativeDir,
        DirectoryPath buildArtifactDir,
        DirectoryPath testResultsOutputDir)
    {
        Artifacts = artifactsDir;
        ArtifactsBin = artifactsBinDir;
        ArtifactsBinCoreFx21 = artifactsBinCoreFx21Dir;
        ArtifactsBinCoreFx30 = artifactsBinCoreFx30Dir;
        ArtifactsBinFullFx472 = artifactsBinFullFx472Dir;
        Native = nativeDir;
        BuildArtifact = buildArtifactDir;
        TestResultsOutput = testResultsOutputDir;
        ToClean = new [] {
            Artifacts,
            ArtifactsBin,
            ArtifactsBinCoreFx21,
            ArtifactsBinCoreFx30,
            ArtifactsBinFullFx472,
            Native,
            BuildArtifact,
            TestResultsOutput
        };
    }
}
