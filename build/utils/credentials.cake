public class BuildCredentials
{
    public CodeCovCredentials CodeCov { get; private set; }

    public static BuildCredentials GetCredentials(ICakeContext context)
    {
        return new BuildCredentials
        {
            CodeCov = CodeCovCredentials.GetCodeCovCredentials(context)
        };
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
