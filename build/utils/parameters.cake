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
    public string CoreFxVersion31 { get; private set; } = "netcoreapp3.1";
    public string NetVersion50 { get; private set; } = "net5.0";
    public string NetVersion60 { get; private set; } = "net6.0";
    public string FullFxVersion472 { get; private set; } = "net472";

    public bool EnabledUnitTests { get; private set; }
    public bool EnabledPublishNuGet { get; private set; }

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
    public Dictionary<PlatformFamily, string[]> NativeRuntimes { get; private set; }

    public bool IsStableRelease() => !IsLocalBuild && IsMasterBranch && IsTagged && !IsPullRequest;
    public bool IsPreviewRelease() => !IsLocalBuild && IsReleaseBranch && IsTagged && !IsPullRequest;
    public bool IsBetaRelease() => !IsLocalBuild && IsDevelopBranch && !IsTagged && !IsPullRequest;

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
            EnabledPublishNuGet = IsEnabled(context, "ENABLED_PUBLISH_NUGET"),

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

        SetMSBuildSettingsVersion(MSBuildSettings, Version);

        Paths = BuildPaths.GetPaths(context, this);

        var buildArtifacts = context.GetFiles(Paths.Directories.BuildArtifact + "/*.*")
            + context.GetFiles(Paths.Directories.ArtifactsNative + "/*.*");
        Artifacts = buildArtifacts.Select(ba => ba.FullPath).ToArray();

        NativeRuntimes = new Dictionary<PlatformFamily, string[]>
        {
            [PlatformFamily.Windows] = new[] { "win-x64", "win-x86" },
            [PlatformFamily.Linux] = new[] { "linux-x64", "linux-musl-x64" },
            [PlatformFamily.OSX] = new[] { "osx-x64" }
        };
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
        msBuildSettings.WithProperty("PackageVersion", version.SemVersion.ToLowerInvariant());
        msBuildSettings.WithProperty("FileVersion", version.Version);
        msBuildSettings.WithProperty("NoPackageAnalysis", "true");
    }

    private static bool IsOnMainRepo(ICakeContext context)
    {
        var buildSystem = context.BuildSystem();
        string repositoryName = null;

        if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryName = buildSystem.AzurePipelines.Environment.Repository.RepoName;

        context.Information("Repository Name: {0}" , repositoryName);

        return !string.IsNullOrWhiteSpace(repositoryName) && StringComparer.OrdinalIgnoreCase.Equals($"{BuildParameters.MainRepoOwner}/{BuildParameters.MainRepoName}", repositoryName);
    }

    private static bool IsOnBranch(ICakeContext context, string branch, bool logBranch = false)
    {
        var buildSystem = context.BuildSystem();
        string repositoryBranch = ExecGitCmd(context, "rev-parse --abbrev-ref HEAD").Single();

        if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
            repositoryBranch = buildSystem.AzurePipelines.Environment.Repository.SourceBranchName;

        if (logBranch)
            context.Information("Repository Branch: {0}", repositoryBranch);

        return !string.IsNullOrWhiteSpace(repositoryBranch) && StringComparer.OrdinalIgnoreCase.Equals(branch, repositoryBranch);
    }

    private static bool IsOnBranchStartingWith(ICakeContext context, string branchPrefix)
    {
        var buildSystem = context.BuildSystem();
        string repositoryBranch = ExecGitCmd(context, "rev-parse --abbrev-ref HEAD").Single();

        if (buildSystem.IsRunningOnAzurePipelines || buildSystem.IsRunningOnAzurePipelinesHosted)
        {
            branchPrefix = branchPrefix.Insert(0, "refs/heads/");
            repositoryBranch = buildSystem.AzurePipelines.Environment.Repository.SourceBranch;
        }

        return !string.IsNullOrWhiteSpace(repositoryBranch) && repositoryBranch.StartsWith(branchPrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBuildTagged(ICakeContext context)
    {
        var sha = ExecGitCmd(context, "rev-parse --verify HEAD").Single();
        return ExecGitCmd(context, "tag --points-at " + sha).Any();
    }
}
