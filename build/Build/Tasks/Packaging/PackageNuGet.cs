// Copyright (c) 2022 DrBarnabus


using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core;
using Cake.Frosting;
using Common.Models;

// ReSharper disable UnusedType.Global

namespace Build.Tasks.Packaging
{
    [TaskName(nameof(PackageNuGet))]
    [TaskDescription("Creates the NuGet Packages")]
    [IsDependentOn(typeof(PackagePrepare))]
    public class PackageNuGet : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.EnsureDirectoryExists(Paths.NuGet);

            var settings = new DotNetCorePackSettings
            {
                Configuration = context.MsBuildConfiguration,
                NoRestore = true,
                OutputDirectory = Paths.NuGet,
                MSBuildSettings = context.MsBuildSettings
            };

            context.DotNetCorePack("./src/DacTools.Deployment.Core/DacTools.Deployment.Core.csproj", settings);

            settings.ArgumentCustomization = arg => arg.Append("/p:PackAsTool=true");
            context.DotNetCorePack("./src/DacTools.Deployment/DacTools.Deployment.csproj", settings);
        }
    }
}
