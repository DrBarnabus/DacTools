// Copyright (c) 2022 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Frosting;
using Common.Models;

// ReSharper disable UnusedType.Global

namespace Build.Tasks
{
    [TaskName(nameof(Clean))]
    [TaskDescription("Cleans build artifacts")]
    public class Clean : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Information("Cleaning directories...");

            context.CleanDirectories(Paths.Src + "/**/bin/" + context.MsBuildConfiguration);
            context.CleanDirectories(Paths.Src + "/**/obj");

            context.CleanDirectory(Paths.TestResults);
            context.CleanDirectory(Paths.Packages);
            context.CleanDirectory(Paths.Native);
            context.CleanDirectory(Paths.FrameworkDependent);
            context.CleanDirectory(Paths.Artifacts);
        }
    }
}
