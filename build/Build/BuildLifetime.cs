﻿// Copyright (c) 2021 DrBarnabus

using Build.Setup;
using Cake.Common;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Common;
using Common.Constants;
using Common.Extensions;
using Common.Models;

namespace Build
{
    public class BuildLifetime : BuildLifetimeBase<BuildContext>
    {
        public override void Setup(BuildContext context)
        {
            base.Setup(context);

            context.MsBuildConfiguration = context.Argument(Arguments.Configuration, "Release");
            context.EnableUnitTests = context.IsEnabled(EnvVars.EnableUnitTests);
            context.Credentials = Credentials.GetCredentials(context);
            SetMsBuildSettingsVersion(context.MsBuildSettings, context.Version!);

            context.StartGroup("Build Setup");
            LogBuildInformation(context);
            LogVariable(context, "Configuration", context.MsBuildConfiguration);
            LogVariable(context, "Enable Unit Tests?", context.EnableUnitTests);
            context.EndGroup();
        }

        private static void SetMsBuildSettingsVersion(DotNetCoreMSBuildSettings msBuildSettings, BuildVersion version)
        {
            msBuildSettings.WithProperty("Version", version.SemVersion);
            msBuildSettings.WithProperty("AssemblyVersion", version.Version);
            msBuildSettings.WithProperty("PackageVersion", version.SemVersion);
            msBuildSettings.WithProperty("FileVersion", version.Version);
            msBuildSettings.WithProperty("InformationalVersion", version.GitVersion.InformationalVersion);
            msBuildSettings.WithProperty("RepositoryBranch", version.GitVersion.BranchName);
            msBuildSettings.WithProperty("RepositoryCommit", version.GitVersion.Sha);
            msBuildSettings.WithProperty("NoPackageAnalysis", "true");
        }
    }
}
