// Install Modules
#module nuget:?package=Cake.DotNetTool.Module&version=0.3.1

// Install addins.
#addin "nuget:?package=Cake.Compression&version=0.2.4"
#addin "nuget:?package=Cake.Incubator&version=5.1.0"

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

    Information("Building version {0} of DacTools ({1}, {2})",
        parameters.Version.SemVersion,
        parameters.Configuration,
        parameters.Target);

    return parameters;
});

Teardown<BuildParameters>((context, parameters) =>
{
    try
    {
        Information("Starting Teardown...");
        Information("Finished running tasks.");
    }
    catch (Exception ex)
    {
        Error(ex.Dump());
    }
});

// Tasks
Task("Pack")
    .IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Pack");

// Execution
var target = Argument("targett", "Default");
RunTarget(target);
