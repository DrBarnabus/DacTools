// Copyright (c) 2022 DrBarnabus

using Build.Tasks.Packaging;
using Cake.Frosting;

// ReSharper disable UnusedType.Global

namespace Build.Tasks
{
    [TaskName(nameof(Package))]
    [TaskDescription("Creates the packages (nuget, tar.gz)")]
    [IsDependentOn(typeof(PackageGZip))]
    [IsDependentOn(typeof(PackageNuGet))]
    public class Package : FrostingTask<BuildContext>
    {
    }
}
