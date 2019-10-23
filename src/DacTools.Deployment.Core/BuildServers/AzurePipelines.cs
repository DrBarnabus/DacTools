using DacTools.Deployment.Core.BuildServers.Common;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.BuildServers
{
	public class AzurePipelines : BuildServerBase
	{
		public AzurePipelines(ILog log) : base(log)
		{
		}

		public const string EnvironmentVariableValue = "TF_BUILD";

		protected override string EnvironmentVariable { get; } = EnvironmentVariableValue;

		public override string GenerateSetProgressMessage(int current, int total, string message) =>
			$"##vso[task.setprogress value={(int) (current / (double) total * 100.0f)};] {message}";

		public override string GenerateLogIssueWarningMessage(string issueMessage) =>
			$"##vso[task.logissue type=warning;] {issueMessage}";

		public override string GenerateLogIssueErrorMessage(string issueMessage) =>
			$"##vso[task.logissue type=error;] {issueMessage}";

		public override string GenerateSetStatusSucceededWithIssuesMessage(string statusMessage) =>
			$"##vso[task.complete type=SucceededWithIssues;] {statusMessage}";

		public override string GenerateSetStatusFailMessage(string statusMessage) =>
			$"##vso[task.complete type=Failed;] {statusMessage}";
	}
}
