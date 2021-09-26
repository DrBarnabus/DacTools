// Copyright (c) 2021 DrBarnabus

namespace Common.Constants
{
    public static class Constants
    {
        public const string RepoOwner = "DrBarnabus";
        public const string RepoName = "DacTools";

        public const string Version31 = "3.1";
        public const string Version50 = "5.0";
        public const string Version60 = "6.0";

        public const string NetCoreVersion31 = "netcoreapp3.1";
        public const string NetVersion50 = "net5.0";
        public const string NetVersion60 = "net6.0";

        public static readonly string[] VersionsToBuild = { NetVersion60, NetVersion50, NetCoreVersion31 };

        public const string NuGetUrl = "https://api.nuget.org/v3/index.json";
        public const string GitHubPackagesUrl = "https://nuget.pkg.github.com/drbarnabus/index.json";
    }
}
