// Install Modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.3.1

// Install addins.
#addin "nuget:?package=Cake.Codecov&version=0.7.0"
#addin "nuget:?package=Cake.Compression&version=0.2.4"
#addin "nuget:?package=Cake.Coverlet&version=2.3.4"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"

#addin "nuget:?package=SharpZipLib&version=1.2.0"

// Install .NET Core Global Tools
#tool "dotnet:?package=Codecov.Tool&version=1.7.2"
#tool "dotnet:?package=GitVersion.Tool&version=5.0.1"

// Load Other Scripts
#load "./build/utils/parameters.cake"
#load "./build/utils/utils.cake"

#load "./build/pack.cake"
#load "./build/publish.cake"

using System.Diagnostics;

// Parameters
bool publishingError = false;

// Setup / Teardown
Setup<BuildParameters>(context =>
{
    EnsureDirectoryExists("artifacts");
    var parameters = BuildParameters.GetParameters(context);
    var gitVersion = GetVersion(parameters);
    parameters.Initialize(context, gitVersion);

    // Increase Verbosity?
    if ((parameters.IsMasterBranch || parameters.IsReleaseBranch || parameters.IsDevelopBranch) && (context.Log.Verbosity != Verbosity.Diagnostic))
    {
        Information("Increasing Verbosity to Diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    Information("Building Version {0} of DacTools ({1}, {2})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target);

    Information("Repository Info: IsMainRepo {0}, IsMasterBranch {1}, IsReleaseBranch {2}, IsDevelopBranch {3}, IsTagged {4}, IsPullRequest: {5}, IsStableRelase {6}, IsPreviewRelease {7}, IsBetaRelease {8}",
        parameters.IsMainRepo,
        parameters.IsMasterBranch,
        parameters.IsReleaseBranch,
        parameters.IsDevelopBranch,
        parameters.IsTagged,
        parameters.IsPullRequest,
        parameters.IsStableRelase(),
        parameters.IsPreviewRelease(),
        parameters.IsBetaRelease());

    return parameters;
});

Teardown<BuildParameters>((context, parameters) =>
{
    try
    {
        Information("Starting Teardown...");

        Information("Repository Info: IsMainRepo {0}, IsMasterBranch {1}, IsReleaseBranch {2}, IsDevelopBranch {3}, IsTagged {4}, IsPullRequest: {5}, IsStableRelase {6}, IsPreviewRelease {7}, IsBetaRelease {8}",
            parameters.IsMainRepo,
            parameters.IsMasterBranch,
            parameters.IsReleaseBranch,
            parameters.IsDevelopBranch,
            parameters.IsTagged,
            parameters.IsPullRequest,
            parameters.IsStableRelase(),
            parameters.IsPreviewRelease(),
            parameters.IsBetaRelease());

        Information("Finished running tasks.");
    }
    catch (Exception ex)
    {
        Error(ex.Dump());
    }
});

// Tasks
Task("Pack")
    .IsDependentOn("Pack-NuGet")
    .IsDependentOn("Zip-Files")
    .IsDependentOn("GZip-Files")
    .ReportError(exception =>
    {
        Error(exception.Dump());
    });

Task("Publish")
    .IsDependentOn("Publish-AzurePipelines")
    .IsDependentOn("Publish-Coverage")
    .IsDependentOn("Publish-NuGet")
    .Finally(() =>
    {
        if (publishingError)
            throw new Exception("An error ocurred during the publishing of DacTools. All publishing tasks have been attempted, but at least one failed with an error.");
    });

Task("Default")
    .IsDependentOn("Pack");

// Execution
var target = Argument("target", "Default");
RunTarget(target);
