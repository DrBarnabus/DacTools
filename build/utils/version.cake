public class BuildVersion
{
    public GitVersion GitVersion { get; private set; }
    public string Version { get; private set; }
    public string SemVersion { get; private set; }

    public static BuildVersion Calculate(ICakeContext context, GitVersion gitVersion)
    {
        var version = gitVersion.MajorMinorPatch;
        var semVersion = gitVersion.SemVer;

        return new BuildVersion
        {
            GitVersion = gitVersion,
            Version = version,
            SemVersion = semVersion
        };
    }
}
