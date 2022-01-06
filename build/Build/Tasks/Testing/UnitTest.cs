// Copyright (c) 2022 DrBarnabus

using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core.IO;
using Cake.Coverlet;
using Cake.Frosting;
using Cake.Incubator.LoggingExtensions;
using Common.Attributes;
using Common.Constants;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Build.Tasks.Testing
{
    [TaskName(nameof(UnitTest))]
    [TaskDescription("Run the unit tests")]
    [TaskArgument(Arguments.DotnetTarget, Constants.NetVersion60, Constants.NetVersion50, Constants.NetCoreVersion31)]
    [IsDependentOn(typeof(Build))]
    public class UnitTest : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.EnableUnitTests;

        public override void Run(BuildContext context)
        {
            string[] frameworks = Constants.VersionsToBuild;

            string dotnetTarget = context.Argument(Arguments.DotnetTarget, string.Empty);
            if (!string.IsNullOrWhiteSpace(dotnetTarget))
            {
                if (!frameworks.Contains(dotnetTarget, StringComparer.OrdinalIgnoreCase))
                    throw new Exception($"Dotnet Target {dotnetTarget} is not supported at the moment");

                frameworks = new[] { dotnetTarget };
            }

            foreach (var framework in frameworks)
            {
                // run using dotnet test
                var projects = context.GetFiles($"{Paths.Test}/**/*.Tests.csproj");
                foreach (var project in projects)
                    TestProjectForTarget(context, project, framework);
            }
        }

        public override void OnError(Exception exception, BuildContext context)
        {
            var innerException = (exception as AggregateException)?.InnerExceptions[0];
            context.Error(innerException.Dump());

            throw exception;
        }

        public override void Finally(BuildContext context)
        {
            var testResultsFiles = context.GetFiles($"{Paths.TestResults}/*.results.xml");
            if (!context.IsAzurePipelinesBuild || !testResultsFiles.Any())
                return;

            context.Information("Found Test Files: {0}", string.Join(",", testResultsFiles));

            var data = new AzurePipelinesPublishTestResultsData
            {
                TestRunTitle = $"Tests_{context.Environment.Platform.Family.ToString()}",
                Platform = context.Environment.Platform.Family.ToString(),
                TestRunner = AzurePipelinesTestRunnerType.VSTest,
                TestResultsFiles = testResultsFiles.ToArray()
            };

            context.BuildSystem().AzurePipelines.Commands.PublishTestResults(data);
        }

        private static void TestProjectForTarget(BuildContext context, FilePath project, string framework)
        {
            string projectName = $"{project.GetFilenameWithoutExtension()}.{framework}";
            var resultsPath = context.MakeAbsolute(Paths.TestResults.CombineWithFilePath($"{projectName}.results.xml"));

            var settings = new DotNetCoreTestSettings
            {
                Framework = framework,
                NoBuild = true,
                NoRestore = true,
                Configuration = context.MsBuildConfiguration,
                TestAdapterPath = new DirectoryPath("."),
                Loggers = new[] { $"trx;LogFileName={resultsPath}" }
            };

            var coverletSettings = new CoverletSettings
            {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.cobertura,
                CoverletOutputDirectory = Paths.TestResults,
                CoverletOutputName = $"{projectName}.coverage.xml"
            };

            context.DotNetCoreTest(project.FullPath, settings, coverletSettings);
        }
    }
}
