#region Build

Task("Clean")
    .Does<BuildParameters>(parameters =>
    {
        Information("Cleaning Directories...");

        // Clean src
        CleanDirectories("./src/**/bin/" + parameters.Configuration);
        CleanDirectories("./src/**/obj");

        // Clean test
        CleanDirectories("./test/**/bin/" + parameters.Configuration);
        CleanDirectories("./test/**/obj");

        CleanDirectories(parameters.Paths.Directories.ToClean);
    });

Task("Build")
    .IsDependentOn("Clean")
    .Does<BuildParameters>(parameters =>
    {
        // Build .NET Code
        var sln = "./DacTools.sln";
        DotNetCoreRestore(sln, new DotNetCoreRestoreSettings
        {
            Verbosity = DotNetCoreVerbosity.Minimal,
            Sources = new [] { "https://api.nuget.org/v3/index.json" },
            MSBuildSettings = parameters.MSBuildSettings
        });

        var slnPath = MakeAbsolute(new DirectoryPath(sln));
        DotNetCoreBuild(slnPath.FullPath, new DotNetCoreBuildSettings
        {
            Verbosity = DotNetCoreVerbosity.Minimal,
            Configuration = parameters.Configuration,
            NoRestore = true,
            MSBuildSettings = parameters.MSBuildSettings
        });
    });

#endregion

#region Tests

Task("Test")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.EnabledUnitTests, "Unit Tests were disabled.")
    .IsDependentOn("Build")
    .Does<BuildParameters>(parameters => {
        var frameworks = new [] { parameters.CoreFxVersion21, parameters.CoreFxVersion30 };
        var testResultsPath = parameters.Paths.Directories.TestResultsOutput;

        foreach (var framework in frameworks)
        {
            // run using dotnet test
            var actions = new List<Action>();
            var projects = GetFiles("./test/**/*.Tests.csproj");

            foreach (var project in projects)
            {
                actions.Add(() =>
                {
                    var projectName = $"{project.GetFilenameWithoutExtension()}.{framework}";
                    var settings = new DotNetCoreTestSettings 
                    {
                        Framework = framework,
                        NoBuild = true,
                        NoRestore = true,
                        Configuration = parameters.Configuration
                    };

                    if (!parameters.IsRunningOnMacOS)
                    {
                        settings.TestAdapterPath = new DirectoryPath(".");
                        var resultsPath = MakeAbsolute(testResultsPath.CombineWithFilePath($"{projectName}.results.xml"));
                        settings.Logger = $"trx;LogFileName={resultsPath}";
                    }

                    DotNetCoreTest(project.FullPath, settings);
                });
            }

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = -1,
                CancellationToken = default
            };

            Parallel.Invoke(options, actions.ToArray());
        }
    })
    .ReportError(exception =>
    {
        var error = (exception as AggregateException).InnerExceptions[0];
        Error(error.Dump());
    })
    .Finally(() =>
    {
        var parameters = Context.Data.Get<BuildParameters>();
        var testResultsFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.results.xml");

        if (parameters.IsRunningOnAzurePipelines)
            if (testResultsFiles.Any())
            {
                var data = new TFBuildPublishTestResultsData
                {
                    TestResultsFiles = testResultsFiles.ToArray(),
                    TestRunner = TFTestRunnerType.VSTest
                };

                TFBuild.Commands.PublishTestResults(data);
            }
    });

#endregion
