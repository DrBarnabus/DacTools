#load "./parameters.cake"

public class BuildPaths
{
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(ICakeContext context, BuildParameters parameters)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        if (parameters is null)
            throw new ArgumentNullException(nameof(parameters));

        var semVersion = parameters.Version.SemVersion;

        var artifactsDir = (DirectoryPath)(context.Directory("./artifacts") + context.Directory("v" + semVersion));
        var artifactsBinDir = artifactsDir.Combine("bin");
        var artifactsNativeDir = ((DirectoryPath)context.Directory("./artifacts")).Combine("native");
        var artifactsBinCoreFx21Dir = artifactsBinDir.Combine(parameters.CoreFxVersion21);
        var artifactsBinCoreFx31Dir = artifactsBinDir.Combine(parameters.CoreFxVersion31);
        var artifactsBinNet51Dir = artifactsBinDir.Combine(parameters.NetVersion50);
        var artifactsBinFullFx472Dir = artifactsBinDir.Combine(parameters.FullFxVersion472);

        var nativeDir = artifactsDir.Combine("native");
        var buildArtifactDir = artifactsDir.Combine("build-artifact");
        var testResultsOutputDir = artifactsDir.Combine("test-results");

        // Directories
        var buildDirectories = new BuildDirectories(
            artifactsDir,
            artifactsBinDir,
            artifactsNativeDir,
            artifactsBinCoreFx21Dir,
            artifactsBinCoreFx31Dir,
            artifactsBinNet51Dir,
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
    public DirectoryPath ArtifactsNative { get; private set; }
    public DirectoryPath ArtifactsBinCoreFx21 { get; private set; }
    public DirectoryPath ArtifactsBinCoreFx31 { get; private set; }
    public DirectoryPath ArtifactsBinNet50 { get; private set; }
    public DirectoryPath ArtifactsBinFullFx472 { get; private set; }
    public DirectoryPath Native { get; private set; }
    public DirectoryPath BuildArtifact { get; private set; }
    public DirectoryPath TestResultsOutput { get; private set; }

    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath artifactsDir,
        DirectoryPath artifactsBinDir,
        DirectoryPath artifactsNativeDir,
        DirectoryPath artifactsBinCoreFx21Dir,
        DirectoryPath artifactsBinCoreFx31Dir,
        DirectoryPath artifactsBinNet50,
        DirectoryPath artifactsBinFullFx472Dir,
        DirectoryPath nativeDir,
        DirectoryPath buildArtifactDir,
        DirectoryPath testResultsOutputDir)
    {
        Artifacts = artifactsDir;
        ArtifactsBin = artifactsBinDir;
        ArtifactsNative = artifactsNativeDir;
        ArtifactsBinCoreFx21 = artifactsBinCoreFx21Dir;
        ArtifactsBinCoreFx31 = artifactsBinCoreFx31Dir;
        ArtifactsBinNet50 = artifactsBinNet50;
        ArtifactsBinFullFx472 = artifactsBinFullFx472Dir;
        Native = nativeDir;
        BuildArtifact = buildArtifactDir;
        TestResultsOutput = testResultsOutputDir;
        ToClean = new [] {
            Artifacts,
            ArtifactsBin,
            ArtifactsNative,
            ArtifactsBinCoreFx21,
            ArtifactsBinCoreFx31,
            ArtifactsBinNet50,
            ArtifactsBinFullFx472,
            Native,
            BuildArtifact,
            TestResultsOutput
        };
    }
}
