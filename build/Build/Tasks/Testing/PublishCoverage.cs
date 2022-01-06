// Copyright (c) 2022 DrBarnabus

using Cake.Codecov;
using Cake.Common.IO;
using Cake.Frosting;
using Common.Extensions;
using Common.Models;
using System;

// ReSharper disable UnusedType.Global

namespace Build.Tasks.Testing
{
    [TaskName(nameof(PublishCoverage))]
    [TaskDescription("Publishes the test coverage")]
    [IsDependentOn(typeof(UnitTest))]
    public class PublishCoverage : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context)
        {
            bool shouldRun = true;

            shouldRun &= context.ShouldRun(context.IsOnLinux, $"{nameof(PublishCoverage)} works only on Linux agents.");
            shouldRun &= context.ShouldRun(context.IsOriginalRepo, $"{nameof(PublishCoverage)} works only on original repository.");
            shouldRun &= context.ShouldRun(!string.IsNullOrEmpty(context.Credentials?.Codecov?.Token), $"{nameof(PublishCoverage)} works only when 'CODECOV_TOKEN' is supplied.");

            return shouldRun;
        }

        public override void Run(BuildContext context)
        {
            var coverageFiles = context.GetFiles($"{Paths.TestResults}/*.coverage.*.xml");

            string? token = context.Credentials?.Codecov?.Token;
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Could not resolve CodeCov token.");

            foreach (var coverageFile in coverageFiles)
                context.Codecov(new CodecovSettings
                {
                    Files = new[] { coverageFile.ToString() },
                    Token = token
                });
        }
    }
}
