// Copyright (c) 2022 DrBarnabus

namespace Common.Constants
{
    public static class Constants
    {
        public const string RepoOwner = "DrBarnabus";
        public const string RepoName = "DacTools";

        public const string Version60 = "6.0";
        public const string Version80 = "8.0";

        public const string NetVersion60 = "net6.0";
        public const string NetVersion80 = "net8.0";

        public static readonly string[] VersionsToBuild = { NetVersion80, NetVersion60 };

        public const string NuGetUrl = "https://api.nuget.org/v3/index.json";
        public const string GitHubPackagesUrl = "https://nuget.pkg.github.com/drbarnabus/index.json";
    }
}
