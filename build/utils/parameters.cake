#load "./paths.cake"
#load "./version.cake"

public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }

    public string CoreFxVersion21 { get; private set; } = "netcoreapp2.1";
    public string CoreFxVersion30 { get; private set; } = "netcoreapp3.0";
    public string FullFxVersion472 {get; private set; } = "net472";

    public bool EnabledUnitTests { get; private set; }

    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnLinux { get; private set; }
    public bool IsRunningOnMacOS { get; private set; }

    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAzurePipelines { get; private set; }

    public bool IsPullRequest { get; private set; }

    public DotNetCoreMSBuildSettings MSBuildSettings { get; private set; }

    public BuildVersion Version { get; private set; }
    public BuildPaths Paths { get; private set; }
    public Dictionary<PlatformFamily, string> NativeRuntimes { get; private set; }

    public static BuildParameters GetParameters(ICakeContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var target = context.Argument("target", "Default");
        var buildSystem = context.BuildSystem();

        return new BuildParameters 
        {
            Target = target,
            Configuration = context.Argument("configuration", "Release"),

            EnabledUnitTests = IsEnabled(context, "ENABLED_UNIT_TESTS"),

            IsRunningOnUnix = context.IsRunningOnUnix(),
            IsRunningOnWindows = context.IsRunningOnWindows(),
            IsRunningOnLinux = context.Environment.Platform.Family == PlatformFamily.Linux,
            IsRunningOnMacOS = context.Environment.Platform.Family == PlatformFamily.OSX,

            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAzurePipelines = buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted,

            IsPullRequest = buildSystem.IsPullRequest,

            MSBuildSettings = GetMSBuildSettings(context)
        };
    }

    public void Initialize(ICakeContext context, GitVersion gitVersion)
    {
        Version = BuildVersion.Calculate(context, gitVersion);

        Paths = BuildPaths.GetPaths(context, this, Configuration, Version);

        NativeRuntimes = new Dictionary<PlatformFamily, string>
        {
            [PlatformFamily.Windows] = "win-x64",
            [PlatformFamily.Linux] = "linux-x64",
            [PlatformFamily.OSX] = "osx-x64"
        };

        SetMSBuildSettingsVersion(MSBuildSettings, Version);
    }

    private static DotNetCoreMSBuildSettings GetMSBuildSettings(ICakeContext context)
    {
        var msBuildSettings = new DotNetCoreMSBuildSettings();

        // Custom Parameters May Be Added Here in the Future

        return msBuildSettings;
    }

    private void SetMSBuildSettingsVersion(DotNetCoreMSBuildSettings msBuildSettings, BuildVersion version)
    {
        msBuildSettings.WithProperty("Version", version.SemVersion);
        msBuildSettings.WithProperty("AssemblyVersion", version.Version);
        msBuildSettings.WithProperty("FileVersion", version.Version);
        msBuildSettings.WithProperty("NoPackageAnalysis", "true");
    }
}
