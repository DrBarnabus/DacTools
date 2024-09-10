// Copyright (c) 2022 DrBarnabus

using Cake.Common.Diagnostics;
using Cake.Frosting;
using Common.Constants;
using Common.Extensions;
using System;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;

// ReSharper disable UnusedType.Global

namespace Publish.Tasks
{
    [TaskName(nameof(PublishNuGet))]
    [TaskDescription("Publish NuGet Packages")]
    [IsDependentOn(typeof(PublishNuGetInternal))]
    public class PublishNuGet : FrostingTask<BuildContext>
    {
    }

    [TaskName(nameof(PublishNuGetInternal))]
    [TaskDescription("Publish NuGet Packages")]
    public class PublishNuGetInternal : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            bool shouldRun = true;

            shouldRun &= context.ShouldRun(context.IsGitHubActionsBuild, $"{nameof(PublishNuGet)} works only on GitHub Actions.");
            shouldRun &= context.ShouldRun(context.IsGitHubActionsBuild, $"{nameof(PublishNuGet)} works only for releases.");

            return shouldRun;
        }

        public override void Run(BuildContext context)
        {
            // Publish to NuGet.org
            PublishToNuGet(context);

            // Publish to GitHub packages
            if (context.IsGitHubActionsBuild)
                PublishToGitHubPackages(context);
        }

        private static void PublishToGitHubPackages(BuildContext context)
        {
            string? apiKey = context.Credentials?.GitHub?.Token;
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Could not resolve NuGet GitHub Packages API Key.");

            PublishToNuGetRepo(context, apiKey, Constants.GitHubPackagesUrl);
        }

        private static void PublishToNuGet(BuildContext context)
        {
            string? apiKey = context.Credentials?.NuGet?.ApiKey;
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Could not resolve NuGet.org API Key.");

            PublishToNuGetRepo(context, apiKey, Constants.NuGetUrl);
        }

        private static void PublishToNuGetRepo(BuildContext context, string apiKey, string apiUrl)
        {
            string? version = context.Version?.SemVersion;
            foreach ((string packageName, var filePath) in context.Packages)
            {
                context.Information($"NuGet Package {packageName} with version {version} is being published to {apiUrl}.");
                context.DotNetNuGetPush(filePath.FullPath, new DotNetNuGetPushSettings
                {
                    ApiKey = apiKey,
                    Source = apiUrl,
                    SkipDuplicate = true
                });
            }
        }
    }
}
