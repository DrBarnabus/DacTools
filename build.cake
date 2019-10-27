// Install Modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.3.1

// Install addins.
#addin "nuget:?package=Cake.Compression&version=0.2.4"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"

#addin "nuget:?package=SharpZipLib&version=1.2.0"

// Install .NET Core Global Tools
#tool "dotnet:?package=GitVersion.Tool&version=5.0.1"

// Load Other Scripts
#load "./build/utils/parameters.cake"
#load "./build/utils/utils.cake"

#load "./build/pack.cake"

using System.Diagnostics;

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

    Information("Repository Info: IsMainRepo {0}, IsMasterBranch {1}, IsReleaseBranch {2}, IsDevelopBranch {3}, IsTagged {4}, IsPullRequest: {5}",
        parameters.IsMainRepo,
        parameters.IsMasterBranch,
        parameters.IsReleaseBranch,
        parameters.IsDevelopBranch,
        parameters.IsTagged,
        parameters.IsPullRequest);

    return parameters;
});

Teardown<BuildParameters>((context, parameters) =>
{
    try
    {
        Information("Starting Teardown...");

        Information("Repository Info: IsMainRepo {0}, IsMasterBranch {1}, IsReleaseBranch {2}, IsDevelopBranch {3}, IsTagged {4}, IsPullRequest: {5}",
            parameters.IsMainRepo,
            parameters.IsMasterBranch,
            parameters.IsReleaseBranch,
            parameters.IsDevelopBranch,
            parameters.IsTagged,
            parameters.IsPullRequest);

        Information("Finished running tasks.");
    }
    catch (Exception ex)
    {
        Error(ex.Dump());
    }
});

// Tasks
Task("Pack")
    .IsDependentOn("Zip-Files")
    .ReportError(exception =>
    {
        Error(exception.Dump());
    });

Task("Default")
    .IsDependentOn("Pack");

// Execution
var target = Argument("target", "Default");
RunTarget(target);
