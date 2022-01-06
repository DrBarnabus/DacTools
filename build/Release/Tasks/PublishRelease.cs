// Copyright (c) 2022 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.GitReleaseManager;
using Cake.Common.Tools.GitReleaseManager.Create;
using Cake.Frosting;
using Common.Constants;
using Common.Extensions;
using Common.Models;
using System;
using System.Linq;

// ReSharper disable UnusedType.Global

namespace Release.Tasks
{
    [TaskName(nameof(PublishRelease))]
    [TaskDescription("Publish Release")]
    [IsDependentOn(typeof(PublishReleaseInternal))]
    public class PublishRelease : FrostingTask<BuildContext>
    {
    }

    [TaskName(nameof(PublishReleaseInternal))]
    [TaskDescription("Publish Release")]
    public class PublishReleaseInternal : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            bool shouldRun = true;

            shouldRun &= context.ShouldRun(context.IsGitHubActionsBuild, $"{nameof(PublishRelease)} works only on GitHub Actions.");
            shouldRun &= context.ShouldRun(context.IsStableBuild, $"{nameof(PublishRelease)} works only for stable releases.");

            return shouldRun;
        }

        public override void Run(BuildContext context)
        {
            string? token = context.Credentials?.GitHub?.Token;
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Could not resolve GitHub Token.");

            var nativeFiles = context.GetFiles(Paths.Native + "/*.tar.gz").Select(x => x.ToString());
            var frameworkFiles = context.GetFiles(Paths.FrameworkDependent + "/*.tar.gz").Select(x => x.ToString());

            var assetFiles = nativeFiles.Concat(frameworkFiles).ToList();
            context.Information("Asset Count: " + assetFiles.Count);

            string assets = string.Join(",", assetFiles);

            string? milestone = context.Version?.Milestone;
            if (milestone is null) return;

            context.GitReleaseManagerCreate(token, Constants.RepoOwner, Constants.RepoName, new GitReleaseManagerCreateSettings
            {
                Milestone = milestone,
                Name = milestone,
                Prerelease = false,
                TargetCommitish = "main"
            });

            context.GitReleaseManagerAddAssets(token, Constants.RepoOwner, Constants.RepoName, milestone, assets);
            context.GitReleaseManagerClose(token, Constants.RepoOwner, Constants.RepoName, milestone);
        }
    }
}
