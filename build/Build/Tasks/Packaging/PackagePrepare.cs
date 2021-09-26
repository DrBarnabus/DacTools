// Copyright (c) 2021 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Publish;
using Cake.Core.IO;
using Cake.Frosting;
using Common.Constants;
using Common.Models;

// ReSharper disable UnusedType.Global

namespace Build.Tasks.Packaging
{
    [TaskName(nameof(PackagePrepare))]
    [TaskDescription("Prepares for packaging")]
    [IsDependentOn(typeof(Build))]
    public class PackagePrepare : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Information("Packing Native Builds...");
            PackPrepareNative(context);

            context.Information("Packing Framework-Dependent Builds...");
            PackPrepareFrameworkDependent(context);
        }

        private static void PackPrepareFrameworkDependent(BuildContext context)
        {
            foreach (string framework in Constants.VersionsToBuild)
            {
                var outputDirectory = Paths.FrameworkDependent.Combine(framework);
                var settings = new DotNetCorePublishSettings
                {
                    Framework = framework,
                    NoRestore = false,
                    Configuration = context.MsBuildConfiguration,
                    OutputDirectory = outputDirectory,
                    MSBuildSettings = context.MsBuildSettings
                };

                context.DotNetCorePublish("./src/DacTools.Deployment/DacTools.Deployment.csproj", settings);

                context.Information("Framework-Dependent Build for {0} created in {1}", framework, outputDirectory);
            }
        }

        private static void PackPrepareNative(BuildContext context)
        {
            // publish single file for all native runtimes (self contained)
            var platform = context.Environment.Platform.Family;
            foreach (string runtime in context.NativeRuntimes[platform])
            {
                var outputDirectory = PackPrepareNative(context, runtime);
                context.Information("Native Build for {0}-{1} created in {2}",
                    platform.ToString().ToLowerInvariant(), runtime, outputDirectory);
            }
        }

        private static DirectoryPath PackPrepareNative(BuildContext context, string runtime)
        {
            var platform = context.Environment.Platform.Family;
            var outputDirectory = Paths.Native.Combine(platform.ToString().ToLowerInvariant()).Combine(runtime);

            var settings = new DotNetCorePublishSettings
            {
                Framework = Constants.NetVersion60,
                Runtime = runtime,
                SelfContained = true,
                PublishSingleFile = false,
                NoRestore = false,
                Configuration = context.MsBuildConfiguration,
                OutputDirectory = outputDirectory,
                MSBuildSettings = context.MsBuildSettings
            };

            context.DotNetCorePublish("./src/DacTools.Deployment/DacTools.Deployment.csproj", settings);

            return outputDirectory;
        }
    }
}
