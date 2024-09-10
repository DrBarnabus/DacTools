// Copyright (c) 2022 DrBarnabus

using Build.Setup;
using Cake.Core;
using Common;
using System.Collections.Generic;
using Cake.Common.Tools.DotNet.MSBuild;

namespace Build
{
    public class BuildContext : BuildContextBase
    {
        public BuildContext(ICakeContext context)
            : base(context)
        {
        }

        public readonly Dictionary<PlatformFamily, string[]> NativeRuntimes = new()
        {
            [PlatformFamily.Windows] = new[] { "win-x64", "win-x86" },
            [PlatformFamily.Linux] = new[] { "linux-x64", "linux-musl-x64" },
            [PlatformFamily.OSX] = new[] { "osx-x64" }
        };

        public bool EnableUnitTests { get; set; }

        public Credentials? Credentials { get; set; }

        public string MsBuildConfiguration { get; set; } = "Release";

        public DotNetMSBuildSettings MsBuildSettings { get; } = new();
    }
}
