#load "../build.cake"

Task("Publish-AzurePipelines")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows, "Publish-AzurePipelines works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipelines, "Publish-AzurePipelines works only on AzurePipelines.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsStableRelease() || parameters.IsBetaRelease() || parameters.IsAlphaRelease(), "Publish-AzurePipelines works only stable, beta or alpha builds.")
    .Does<BuildParameters>(parameters =>
    {
        foreach (var artifact in parameters.Artifacts)
            if (FileExists(artifact))
                TFBuild.Commands.UploadArtifact("", artifact, "artifacts");
    })
    .OnError(exception =>
    {
        Information("Publish-AzurePipelines Task failed, but continuing with next Task...");
        Error(exception.Dump());
        publishingError = true;
    });

Task("Publish-Coverage")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows, "Publish-Coverage works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipelines, "Publish-Coverage works only on AzurePipelines.")
    .Does<BuildParameters>(parameters =>
    {
        var coverageFiles = GetFiles(parameters.Paths.Directories.TestResultsOutput + "/*.coverage.xml");

        var token = parameters.Credentials.CodeCov.Token;
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException("Could not resolve CodeCov token.");

        foreach (var coverageFile in coverageFiles)
        {
            Codecov(new CodecovSettings
            {
                Files = new [] { coverageFile.ToString() },
                Token = token
            });
        }
    })
    .OnError(exception =>
    {
        Information("Publish-Coverage Task Failed, but continuing with next Task...");
        Error(exception.Dump());
        publishingError = true;
    });
