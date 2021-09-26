// Copyright (c) 2021 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Frosting;
using Common.Constants;

// ReSharper disable UnusedType.Global

namespace Build.Tasks
{
    [TaskName(nameof(Build))]
    [TaskDescription("Builds the solution")]
    [IsDependentOn(typeof(Clean))]
    public class Build : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Information("Building Solution...");

            const string sln = "./DacTools.sln";
            context.DotNetCoreRestore(sln, new DotNetCoreRestoreSettings
            {
                Verbosity = DotNetCoreVerbosity.Minimal,
                Sources = new[] { Constants.NuGetUrl },
                MSBuildSettings = context.MsBuildSettings
            });

            context.DotNetCoreBuild(sln, new DotNetCoreBuildSettings
            {
                Verbosity = DotNetCoreVerbosity.Minimal,
                Configuration = context.MsBuildConfiguration,
                NoRestore = true,
                MSBuildSettings = context.MsBuildSettings
            });
        }
    }
}
