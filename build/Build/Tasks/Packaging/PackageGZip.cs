// Copyright (c) 2022 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Compression;
using Cake.Frosting;
using Common.Constants;
using Common.Models;

// ReSharper disable UnusedType.Global

namespace Build.Tasks.Packaging
{
    [TaskName(nameof(PackageGZip))]
    [TaskDescription("Creates the tar.gz Packages")]
    [IsDependentOn(typeof(PackagePrepare))]
    public class PackageGZip : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.EnsureDirectoryExists(Paths.Native);

            var platform = context.Environment.Platform.Family;
            foreach (var runtime in context.NativeRuntimes[platform])
            {
                var sourceDir = Paths.Native.Combine(platform.ToString().ToLowerInvariant()).Combine(runtime);
                var targetDir = Paths.Native;

                context.EnsureDirectoryExists(targetDir);

                string? fileName = $"dactools-deployment-{runtime}-{context.Version?.SemVersion}.tar.gz".ToLowerInvariant();
                var tarFile = targetDir.CombineWithFilePath(fileName);
                var filePaths = context.GetFiles($"{sourceDir}/**/*");
                context.GZipCompress(sourceDir, tarFile, filePaths, 9);

                context.Information($"Created {tarFile}");
            }

            foreach (var framework in Constants.VersionsToBuild)
            {
                var sourceDir = Paths.FrameworkDependent.Combine(framework);
                var targetDir = Paths.FrameworkDependent;

                context.EnsureDirectoryExists(targetDir);

                string? fileName = $"dactools-deployment-{framework}-{context.Version?.SemVersion}.tar.gz".ToLowerInvariant();
                var tarFile = targetDir.CombineWithFilePath(fileName);
                var filePaths = context.GetFiles($"{sourceDir}/**/*");
                context.GZipCompress(sourceDir, tarFile, filePaths, 9);

                context.Information($"Created {tarFile}");
            }
        }
    }
}
