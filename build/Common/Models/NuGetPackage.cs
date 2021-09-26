// Copyright (c) 2021 DrBarnabus

using Cake.Core.IO;

namespace Common.Models
{
    public record NuGetPackage(string PackageName, FilePath FilePath);
}
