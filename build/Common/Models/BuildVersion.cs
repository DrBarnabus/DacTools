// Copyright (c) 2022 DrBarnabus

using Common.Addins.GitVersion;

namespace Common.Models
{
    public record BuildVersion(GitVersion GitVersion, string? Version, string? Milestone, string? SemVersion)
    {
        public static BuildVersion Calculate(GitVersion gitVersion)
        {
            string? version = gitVersion.MajorMinorPatch;
            string? semVersion = gitVersion.SemVer;

            if (!string.IsNullOrWhiteSpace(gitVersion.BuildMetaData))
                semVersion += $"-{gitVersion.BuildMetaData}";

            return new BuildVersion(gitVersion, version, version, semVersion);
        }
    }
}
