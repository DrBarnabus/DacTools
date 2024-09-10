// Copyright (c) 2022 DrBarnabus

using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Core;
using Cake.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{
    public static class CakeContextExtensions
    {
        public static IEnumerable<string> ExecuteCommand(this ICakeContext context, FilePath exe, string? args, DirectoryPath? workDir = null)
        {
            var processSettings = new ProcessSettings { Arguments = args, RedirectStandardOutput = true };
            if (workDir is not null)
                processSettings.WorkingDirectory = workDir;

            context.StartProcess(exe, processSettings, out var redirectedOutput);
            return redirectedOutput.ToList();
        }

        private static IEnumerable<string> ExecGitCmd(this ICakeContext context, string? cmd, DirectoryPath? workDir = null)
        {
            var gitExe = context.Tools.Resolve(context.IsRunningOnWindows() ? "git.exe" : "git");
            return context.ExecuteCommand(gitExe, cmd, workDir);
        }

        public static bool IsOriginalRepo(this ICakeContext context)
        {
            string repoName = context.GetRepoName();
            string originalRepoName = $"{Constants.Constants.RepoOwner}/{Constants.Constants.RepoName}";
            return !string.IsNullOrWhiteSpace(repoName) && StringComparer.OrdinalIgnoreCase.Equals(originalRepoName, repoName);
        }

        public static string GetRepoName(this ICakeContext context)
        {
            var buildSystem = context.BuildSystem();

            if (buildSystem.IsRunningOnGitHubActions)
                return buildSystem.GitHubActions.Environment.Workflow.Repository;

            return "Unavailable";
        }

        public static bool IsBranch(this ICakeContext context, string branchName)
        {
            string repositoryBranch = context.GetBranch();
            return !string.IsNullOrWhiteSpace(repositoryBranch) && repositoryBranch.Equals(branchName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsBranchWithPrefix(this ICakeContext context, string branchPrefix)
        {
            string repositoryBranch = context.GetBranch();
            return !string.IsNullOrWhiteSpace(repositoryBranch) && repositoryBranch.StartsWith(branchPrefix, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetBranch(this ICakeContext context)
        {
            var buildSystem = context.BuildSystem();

            if (buildSystem.IsRunningOnGitHubActions)
                return buildSystem.GitHubActions.Environment.Workflow.Ref.Replace("refs/heads/", string.Empty);

            return context.ExecGitCmd("rev-parse --abbrev-ref HEAD").Single();
        }

        public static bool IsTagged(this ICakeContext context)
        {
            string sha = context.ExecGitCmd("rev-parse --verify HEAD").Single();
            return context.ExecGitCmd("tag --points-at " + sha).Any();
        }

        public static bool IsEnabled(this ICakeContext context, string envVar, bool nullOrEmptyAsEnabled = true)
        {
            string value = context.EnvironmentVariable(envVar);
            return string.IsNullOrWhiteSpace(value) ? nullOrEmptyAsEnabled : bool.Parse(value);
        }

        public static bool ShouldRun(this ICakeContext context, bool criteria, string skipMessage)
        {
            if (criteria) return true;

            context.Information(skipMessage);
            return false;
        }

        public static string GetOs(this ICakeContext context)
        {
            if (context.IsRunningOnWindows()) return "Windows";
            if (context.IsRunningOnLinux()) return "Linux";
            if (context.IsRunningOnMacOs()) return "macOs";
            return string.Empty;
        }

        public static string GetBuildAgent(this ICakeContext context)
        {
            var buildSystem = context.BuildSystem();
            return buildSystem.Provider switch
            {
                BuildProvider.Local => "Local",
                BuildProvider.GitHubActions => "GitHubActions",
                _ => string.Empty
            };
        }

        public static void StartGroup(this ICakeContext context, string title)
        {
            var buildSystem = context.BuildSystem();

            string startGroup = "[group]";
            if (buildSystem.IsRunningOnGitHubActions)
                startGroup = "::group::";

            context.Information($"{startGroup}{title}");
        }

        public static void EndGroup(this ICakeContext context)
        {
            var buildSystem = context.BuildSystem();

            string endgroup = "[endgroup]";
            if (buildSystem.IsRunningOnGitHubActions)
                endgroup = "::endgroup::";

            context.Information($"{endgroup}");
        }
    }
}
