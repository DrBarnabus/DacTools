// Copyright (c) 2022 DrBarnabus

using Cake.Common.IO;
using Common;
using Common.Extensions;
using Common.Models;
using Publish.Setup;

namespace Publish
{
    public class BuildLifetime : BuildLifetimeBase<BuildContext>
    {
        public override void Setup(BuildContext context)
        {
            base.Setup(context);

            context.Credentials = Credentials.GetCredentials(context);

            if (context.Version?.SemVersion != null)
            {
                string? version = context.Version.SemVersion;

                var packageFiles = context.GetFiles(Paths.NuGet + "/*.nupkg");
                foreach (var packageFile in packageFiles)
                {
                    string packageName = packageFile.GetFilenameWithoutExtension().ToString()[..^(version.Length + 1)].ToLowerInvariant();
                    context.Packages.Add(new NuGetPackage(packageName, packageFile));
                }
            }

            context.StartGroup("Build Setup");
            LogBuildInformation(context);
            context.EndGroup();
        }
    }
}
