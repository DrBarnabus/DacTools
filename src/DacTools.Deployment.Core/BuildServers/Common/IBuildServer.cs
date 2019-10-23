namespace DacTools.Deployment.Core.BuildServers.Common
{
	public interface IBuildServer
	{
		bool CanApplyToCurrentContext();
		string GenerateSetProgressMessage(int current, int total, string message);
		string GenerateLogIssueWarningMessage(string issueMessage);
		string GenerateLogIssueErrorMessage(string issueMessage);
		string GenerateSetStatusSucceededWithIssuesMessage(string statusMessage);
		string GenerateSetStatusFailMessage(string statusMessage);
	}
}
