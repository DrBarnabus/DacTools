// Copyright (c) 2022 DrBarnabus

using Cake.Core.IO;

namespace Common.Models
{
    public record NuGetPackage(string PackageName, FilePath FilePath);
}
