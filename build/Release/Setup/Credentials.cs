// Copyright (c) 2021 DrBarnabus

using Cake.Common;
using Cake.Core;
using Common.Models;

namespace Release.Setup
{
    public class Credentials
    {
        public GitHubCredentials? GitHub { get; private set; }

        public static Credentials GetCredentials(ICakeContext context) => new()
        {
            GitHub = new GitHubCredentials(context.EnvironmentVariable("GITHUB_TOKEN"))
        };
    }
}
