// Copyright (c) 2022 DrBarnabus

using Cake.Common;
using Cake.Core;
using Common.Models;

namespace Build.Setup
{
    public class Credentials
    {
        public CodecovCredentials? Codecov { get; private set; }

        public static Credentials GetCredentials(ICakeContext context) => new()
        {
            Codecov = new CodecovCredentials(context.EnvironmentVariable("CODECOV_TOKEN"))
        };
    }
}
