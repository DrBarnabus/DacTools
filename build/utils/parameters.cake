#load "./credentials.cake"
#load "./paths.cake"
#load "./version.cake"
#load "./utils.cake"

public class BuildParameters
{
    public string Target { get; private set; }
    public string Configuration { get; private set; }

    public const string MainRepoOwner = "DrBarnabus";
    public const string MainRepoName = "DacTools";

    public string CoreFxVersion21 { get; private set; } = "netcoreapp2.1";
    public string CoreFxVersion30 { get; private set; } = "netcoreapp3.0";
    public string FullFxVersion472 { get; private set; } = "net472";

    public bool EnabledUnitTests { get; private set; }

    public bool IsRunningOnUnix { get; private set; }
    public bool IsRunningOnWindows { get; private set; }
    public bool IsRunningOnLinux { get; private set; }
    public bool IsRunningOnMacOS { get; private set; }
    public PlatformFamily PlatformFamily { get; private set; }

    public bool IsLocalBuild { get; private set; }
    public bool IsRunningOnAzurePipelines { get; private set; }

    public bool IsMainRepo { get; private set; }
    public bool IsMasterBranch { get; private set; }
    public bool IsReleaseBranch { get; private set; }
    public bool IsDevelopBranch { get; private set; }
    public bool IsTagged { get; private set; }
    public bool IsPullRequest { get; private set; }

    public DotNetCoreMSBuildSettings MSBuildSettings { get; private set; }

    public BuildCredentials Credentials { get; private set; }
    public BuildVersion Version { get; private set; }
    public BuildPaths Paths { get; private set; }
    public string[] Artifacts { get; private set; }
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
            PlatformFamily = context.Environment.Platform.Family,

            IsLocalBuild = buildSystem.IsLocalBuild,
            IsRunningOnAzurePipelines = buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted,

            IsMainRepo = IsOnMainRepo(context),
            IsMasterBranch = IsOnBranch(context, "master", true),
            IsReleaseBranch = IsOnBranchStartingWith(context, "release"),
            IsDevelopBranch = IsOnBranch(context, "develop"),
            IsTagged = IsBuildTagged(context),
            IsPullRequest = buildSystem.IsPullRequest,

            MSBuildSettings = GetMSBuildSettings(context)
        };
    }

    public void Initialize(ICakeContext context, GitVersion gitVersion)
    {
        Credentials = BuildCredentials.GetCredentials(context);

        Version = BuildVersion.Calculate(context, gitVersion);

        Paths = BuildPaths.GetPaths(context, this, Configuration, Version);

        var buildArtifacts = context.GetFiles(Paths.Directories.BuildArtifact + "/*.*");
        Artifacts = buildArtifacts.Select(ba => ba.FullPath).ToArray();

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

        if (!context.IsRunningOnWindows())
            msBuildSettings.WithProperty("ExcludeFramework", "true");

        return msBuildSettings;
    }

    private void SetMSBuildSettingsVersion(DotNetCoreMSBuildSettings msBuildSettings, BuildVersion version)
    {
        msBuildSettings.WithProperty("Version", version.SemVersion);
        msBuildSettings.WithProperty("AssemblyVersion", version.Version);
        msBuildSettings.WithProperty("FileVersion", version.Version);
        msBuildSettings.WithProperty("NoPackageAnalysis", "true");
    }

    private static bool IsOnMainRepo(ICakeContext context)
    {
        var buildSystem = context.BuildSystem();
        string repositoryName = null;

        if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryName = buildSystem.TFBuild.Environment.Repository.RepoName;

        context.Information("Repository Name: {0}" , repositoryName);

        return !string.IsNullOrWhiteSpace(repositoryName) && StringComparer.OrdinalIgnoreCase.Equals($"{BuildParameters.MainRepoOwner}/{BuildParameters.MainRepoName}", repositoryName);
    }

    private static bool IsOnBranch(ICakeContext context, string branch, bool logBranch = false)
    {
        var buildSystem = context.BuildSystem();
        string repositoryBranch = ExecGitCmd(context, "rev-parse --abbrev-ref HEAD").Single();

        if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryBranch = buildSystem.TFBuild.Environment.Repository.Branch;

        if (logBranch)
            context.Information("Repository Branch: {0}", repositoryBranch);

        return !string.IsNullOrWhiteSpace(repositoryBranch) && StringComparer.OrdinalIgnoreCase.Equals(branch, repositoryBranch);
    }

    private static bool IsOnBranchStartingWith(ICakeContext context, string branchPrefix)
    {
        var buildSystem = context.BuildSystem();
        string repositoryBranch = ExecGitCmd(context, "rev-parse --abbrev-ref HEAD").Single();

        if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryBranch = buildSystem.TFBuild.Environment.Repository.Branch;

        return !string.IsNullOrWhiteSpace(repositoryBranch) && repositoryBranch.StartsWith(branchPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBuildTagged(ICakeContext context)
    {
        var sha = ExecGitCmd(context, "rev-parse --verify HEAD").Single();
        return ExecGitCmd(context, "tag --points-at " + sha).Any();
    }
}
