public class BuildCredentials
{
    public NuGetCredentials NuGet { get; private set; }
    public CodeCovCredentials CodeCov { get; private set; }

    public static BuildCredentials GetCredentials(ICakeContext context)
    {
        return new BuildCredentials
        {
            NuGet = NuGetCredentials.GetNuGetCredentials(context),
            CodeCov = CodeCovCredentials.GetCodeCovCredentials(context)
        };
    }
}

public class NuGetCredentials
{
    public string ApiKey { get; private set; }
    public string ApiUrl { get; private set; }

    public NuGetCredentials(string apiKey, string apiUrl)
    {
        ApiKey = apiKey;
        ApiUrl = apiUrl;
    }

    public static NuGetCredentials GetNuGetCredentials(ICakeContext context)
    {
        return new NuGetCredentials(context.EnvironmentVariable("NUGET_API_KEY"), context.EnvironmentVariable("NUGET_API_URL"));
    }
}

public class CodeCovCredentials
{
    public string Token { get; private set; }

    public CodeCovCredentials(string token)
    {
        Token = token;
    }

    public static CodeCovCredentials GetCodeCovCredentials(ICakeContext context)
    {
        return new CodeCovCredentials(context.EnvironmentVariable("CODECOV_TOKEN"));
    }
}
