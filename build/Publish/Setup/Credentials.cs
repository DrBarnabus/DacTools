// Copyright (c) 2022 DrBarnabus

using Cake.Common;
using Cake.Core;
using Common.Models;

namespace Publish.Setup
{
    public class Credentials
    {
        public GitHubCredentials? GitHub { get; private set; }

        public NuGetCredentials? NuGet { get; private set; }

        public static Credentials GetCredentials(ICakeContext context) => new()
        {
            GitHub = new GitHubCredentials(context.EnvironmentVariable("GITHUB_TOKEN")),
            NuGet = new NuGetCredentials(context.EnvironmentVariable("NUGET_API_KEY"))
        };
    }
}
