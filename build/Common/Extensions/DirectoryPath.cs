// Copyright (c) 2022 DrBarnabus

using Cake.Core.IO;
using System.IO;

namespace Common.Extensions
{
    public static class DirectoryPathExtensions
    {
        public static DirectoryPath GetRootDirectory()
        {
            var currentPath = DirectoryPath.FromString(Directory.GetCurrentDirectory());
            while (!Directory.Exists(currentPath.Combine(".git").FullPath))
                currentPath = DirectoryPath.FromString(Directory.GetParent(currentPath.FullPath)?.FullName);

            return currentPath;
        }
    }
}
