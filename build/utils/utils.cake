FilePath FindToolInPath(string tool)
{
    var pathEnv = EnvironmentVariable("PATH");
    if (string.IsNullOrEmpty(pathEnv) || string.IsNullOrEmpty(tool)) return tool;

    var paths = pathEnv.Split(new [] { IsRunningOnUnix() ? ':' : ';' }, StringSplitOptions.RemoveEmptyEntries);
    return paths.Select(path => new DirectoryPath(path).CombineWithFilePath(tool)).FirstOrDefault(FilePath => FileExists(FilePath.FullPath));
}

GitVersion GetVersion(BuildParameters parameters)
{
    var settings = new GitVersionSettings
    {
        OutputType = GitVersionOutput.Json,
    };

    var gitVersion = GitVersion(settings);

    if (!parameters.IsLocalBuild && !(parameters.IsRunningOnAzurePipelines && parameters.IsPullRequest))
    {
        settings.UpdateAssemblyInfo = true;
        settings.LogFilePath = "console";
        settings.OutputType = GitVersionOutput.BuildServer;

        GitVersion(settings);
    }

    return gitVersion;
}

public static bool IsEnabled(ICakeContext context, string envVar, bool nullOrEmptyAsEnabled = true)
{
    var value = context.EnvironmentVariable(envVar);
    return string.IsNullOrEmpty(value) ? nullOrEmptyAsEnabled : bool.Parse(value);
}
