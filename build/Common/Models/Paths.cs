// Copyright (c) 2021 DrBarnabus

using Cake.Core.IO;

namespace Common.Models
{
    public static class Paths
    {
        public static readonly DirectoryPath Root = "./";

        public static readonly DirectoryPath Artifacts = Root.Combine("artifacts");
        public static readonly DirectoryPath TestResults = Artifacts.Combine("test-results");
        public static readonly DirectoryPath Packages = Artifacts.Combine("packages");

        public static readonly DirectoryPath Native = Packages.Combine("native");
        public static readonly DirectoryPath FrameworkDependent = Packages.Combine("framework-dependent");
        public static readonly DirectoryPath NuGet = Packages.Combine("nuget");


        public static readonly DirectoryPath Src = Root.Combine("src");
        public static readonly DirectoryPath Test = Root.Combine("test");
        public static readonly DirectoryPath Build = Root.Combine("build");
    }
}
