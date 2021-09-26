// Copyright (c) 2021 DrBarnabus

using Cake.Core;
using Common;
using Common.Models;
using Publish.Setup;
using System.Collections.Generic;

namespace Publish
{
    public class BuildContext : BuildContextBase
    {
        public BuildContext(ICakeContext context)
            : base(context)
        {
        }

        public Credentials? Credentials { get; set; }

        public List<NuGetPackage> Packages { get; set; } = new();
    }
}
