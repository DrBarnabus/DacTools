#load "../build.cake"

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
    .Does<BuildParameters>(parameters =>
    {
        var frameworks = new List<string> { parameters.CoreFxVersion31, parameters.NetVersion50, parameters.NetVersion60 };
        if (parameters.IsRunningOnWindows)
            frameworks.Add(parameters.FullFxVersion472);

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

                    var coverletSettings = new CoverletSettings
                    {
                        CollectCoverage = true,
                        CoverletOutputFormat = CoverletOutputFormat.opencover,
                        CoverletOutputDirectory = testResultsPath,
                        CoverletOutputName = $"{projectName}.coverage.xml"
                    };

                    DotNetCoreTest(project.FullPath, settings, coverletSettings);
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
        if (parameters.IsRunningOnAzurePipelines)
        {
            var testResultsFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.results.xml");
            if (testResultsFiles.Any())
            {
                var data = new AzurePipelinesPublishTestResultsData
                {
                    TestRunTitle = $"Tests_{parameters.Configuration}_{parameters.PlatformFamily.ToString()}",
                    TestResultsFiles = testResultsFiles.ToArray(),
                    TestRunner = AzurePipelinesTestRunnerType.VSTest
                };

                AzurePipelines.Commands.PublishTestResults(data);
            }
        }
    });

#endregion

#region Pack

Task("Pack-Prepare")
    .IsDependentOn("Test")
    .Does<BuildParameters>(parameters =>
    {
        var platform = Context.Environment.Platform.Family;
        var runtimes = parameters.NativeRuntimes[platform];
        foreach (var runtime in runtimes)
        {
            var settings = new DotNetCorePublishSettings
            {
                Framework = parameters.NetVersion60,
                Runtime = runtime,
                NoRestore = false,
                Configuration = parameters.Configuration,
                OutputDirectory = parameters.Paths.Directories.Native.Combine(runtime),
                MSBuildSettings = parameters.MSBuildSettings
            };

            DotNetCorePublish("./src/DacTools.Deployment/DacTools.Deployment.csproj", settings);
        }

        var frameworks = new List<string> { parameters.CoreFxVersion31, parameters.NetVersion50, parameters.NetVersion60 };
        if (parameters.IsRunningOnWindows)
            frameworks.Add(parameters.FullFxVersion472);

        // Publish Framework-Dependent Deployment
        foreach (var framework in frameworks)
        {
            var settings = new DotNetCorePublishSettings
            {
                Framework = framework,
                NoRestore = false,
                Configuration = parameters.Configuration,
                OutputDirectory = parameters.Paths.Directories.ArtifactsBin.Combine(framework),
                MSBuildSettings = parameters.MSBuildSettings
            };

            DotNetCorePublish("./src/DacTools.Deployment/DacTools.Deployment.csproj", settings);
        }
    });

Task("Pack-NuGet")
    .IsDependentOn("Pack-Prepare")
    .Does<BuildParameters>(parameters =>
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = parameters.Configuration,
            NoRestore = true,
            OutputDirectory = parameters.Paths.Directories.BuildArtifact,
            MSBuildSettings = parameters.MSBuildSettings
        };

        DotNetCorePack("./src/DacTools.Deployment.Core/DacTools.Deployment.Core.csproj", settings);

        // dotnet Global Tool
        settings.ArgumentCustomization = arg => arg.Append("/p:PackAsTool=true");
        DotNetCorePack("./src/DacTools.Deployment/DacTools.Deployment.csproj", settings);
    });

Task("Zip-Files")
    .IsDependentOn("Pack-Prepare")
    .Does<BuildParameters>(parameters =>
    {
        var platform = Context.Environment.Platform.Family;
        var runtimes = parameters.NativeRuntimes[platform];
        foreach (var runtime in runtimes)
        {
            var sourceDir = parameters.Paths.Directories.Native.Combine(runtime);
            var targetDir = parameters.Paths.Directories.ArtifactsNative;
            EnsureDirectoryExists(targetDir);

            var fileName = $"dactools-deployment-{runtime}-{parameters.Version.SemVersion}.tar.gz".ToLower();
            var tarFile = targetDir.CombineWithFilePath(fileName);
            var filePaths = GetFiles($"{sourceDir}/**/*");
            GZipCompress(sourceDir, tarFile, filePaths);
        }

        var frameworks = new List<string> { parameters.CoreFxVersion31, parameters.NetVersion50, parameters.NetVersion60 };
        if (parameters.IsRunningOnWindows)
            frameworks.Add(parameters.FullFxVersion472);

        foreach (var framework in frameworks)
        {
            var sourceDir = parameters.Paths.Directories.ArtifactsBin.Combine(framework);
            var targetDir = parameters.Paths.Directories.BuildArtifact;
            EnsureDirectoryExists(targetDir);

            var fileName = $"dactools-deployment-{framework}-{parameters.Version.SemVersion}.tar.gz".ToLower();
            var tarFile = targetDir.CombineWithFilePath(fileName);
            var filePaths = GetFiles($"{sourceDir}/**/*");
            GZipCompress(sourceDir, tarFile, filePaths);
        }
    });

#endregion
