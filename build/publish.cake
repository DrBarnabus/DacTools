Task("Publish-AzurePipelines")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnWindows, "Publish-AzurePipelines works only on Windows agents.")
    .WithCriteria<BuildParameters>((context, parameters) => parameters.IsRunningOnAzurePipelines, "Publish-AzurePipelines works only on AzurePipelines.")
    .WithCriteria<BuildParameters>((context, parameters) => !parameters.IsPullRequest, "Publish-AzurePipelines works only for non-PR commits.")
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
