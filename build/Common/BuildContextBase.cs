// Copyright (c) 2021 DrBarnabus

using Cake.Core;
using Cake.Frosting;
using Common.Models;

namespace Common
{
    public class BuildContextBase : FrostingContext
    {
        protected BuildContextBase(ICakeContext context)
            : base(context)
        {
        }

        public BuildVersion? Version { get; set; }

        public string? RepoName { get; set; }

        public string? BranchName { get; set; }

        public bool IsOriginalRepo { get; set; }

        public bool IsPullRequest { get; set; }

        public bool IsTagged { get; set; }

        public bool IsMainBranch { get; set; }

        public bool IsHotfixBranch { get; set; }

        public bool IsReleaseBranch { get; set; }

        public bool IsDevelopBranch { get; set; }

        public bool IsApplicableBuild => !IsLocalBuild && IsOriginalRepo && !IsPullRequest;

        public bool IsStableBuild => IsApplicableBuild && IsMainBranch && IsTagged;

        public bool IsPreviewBuild => IsApplicableBuild && (IsHotfixBranch || IsReleaseBranch) && !IsTagged;

        public bool IsBetaBuild => IsApplicableBuild && IsDevelopBranch && !IsTagged;

        public bool IsLocalBuild { get; set; }

        public bool IsAzurePipelinesBuild { get; set; }

        public bool IsGitHubActionsBuild { get; set; }

        public bool IsOnWindows { get; set; }

        public bool IsOnLinux { get; set; }

        public bool IsOnMacOs { get; set; }
    }
}
