// Copyright (c) 2022 DrBarnabus

using Cake.Core;
using Common;
using Release.Setup;

namespace Release
{
    public class BuildContext : BuildContextBase
    {
        public BuildContext(ICakeContext context)
            : base(context)
        {
        }

        public Credentials? Credentials { get; set; }
    }
}
