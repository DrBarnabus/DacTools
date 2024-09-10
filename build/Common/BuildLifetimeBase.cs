// Copyright (c) 2022 DrBarnabus

using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Frosting;
using Cake.Incubator.LoggingExtensions;
using Common.Addins.GitVersion;
using Common.Extensions;
using Common.Models;
using System;
using System.Collections.Generic;

namespace Common
{
    public class BuildLifetimeBase<TContext> : FrostingLifetime<TContext>
        where TContext : BuildContextBase
    {
        public override void Setup(TContext context, ISetupContext setupContext)
        {
            context.Version = BuildVersion.Calculate(context.GitVersion(new GitVersionSettings
            {
                LogFilePath = "console",
                OutputTypes = new HashSet<GitVersionOutput> { GitVersionOutput.Json, GitVersionOutput.BuildServer }
            }));

            var buildSystem = context.BuildSystem();

            context.RepoName = context.GetRepoName();
            context.BranchName = context.GetBranch();
            context.IsOriginalRepo = context.IsOriginalRepo();
            context.IsPullRequest = buildSystem.IsPullRequest;
            context.IsTagged = context.IsTagged();
            context.IsMainBranch = context.IsBranch("main");
            context.IsHotfixBranch = context.IsBranchWithPrefix("hotfix");
            context.IsReleaseBranch = context.IsBranchWithPrefix("release");
            context.IsDevelopBranch = context.IsBranch("develop");

            context.IsLocalBuild = buildSystem.IsLocalBuild;
            context.IsGitHubActionsBuild = buildSystem.IsRunningOnGitHubActions;

            context.IsOnWindows = context.IsRunningOnWindows();
            context.IsOnLinux = context.IsRunningOnLinux();
            context.IsOnMacOs = context.IsRunningOnMacOs();
        }

        public override void Teardown(TContext context, ITeardownContext info)
        {
            context.StartGroup("Build Teardown");

            try
            {
                context.Information("Starting Teardown...");

                LogBuildInformation(context);

                context.Information("Finished running tasks.");
            }
            catch (Exception ex)
            {
                context.Error(ex.Dump());
            }

            context.EndGroup();
        }

        protected static void LogVariable(TContext context, string message, object? value)
        {
            context.Information($"{message + ":",-20} {value,30}");
        }

        protected void LogBuildInformation(TContext context)
        {
            LogVariable(context, "Build Version", context.Version?.SemVersion);
            LogVariable(context, "Current Repo", context.RepoName);
            LogVariable(context, "Current Branch", context.BranchName);
            LogVariable(context, "Original Repo?", context.IsOriginalRepo);
            LogVariable(context, "Pull Request?", context.IsPullRequest);
            LogVariable(context, "Tagged?", context.IsTagged);
            LogVariable(context, "Main Branch?", context.IsMainBranch);
            LogVariable(context, "Hotfix/* Branch?", context.IsHotfixBranch);
            LogVariable(context, "Release/* Branch?", context.IsReleaseBranch);
            LogVariable(context, "Develop Branch?", context.IsDevelopBranch);
            LogVariable(context, "Build Agent", context.GetBuildAgent());
            LogVariable(context, "Operating System", context.GetOs());
        }
    }
}
