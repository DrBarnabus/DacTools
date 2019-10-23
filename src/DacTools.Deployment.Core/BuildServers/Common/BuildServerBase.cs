using System;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.BuildServers.Common
{
	public abstract class BuildServerBase : IBuildServer
	{
		protected readonly ILog Log;

		protected BuildServerBase(ILog log)
		{
			Log = log;
		}

		protected abstract string EnvironmentVariable { get; }

		public virtual bool CanApplyToCurrentContext() =>
			!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariable));

		public abstract string GenerateSetProgressMessage(int current, int total, string message);
		public abstract string GenerateLogIssueWarningMessage(string issueMessage);
		public abstract string GenerateLogIssueErrorMessage(string issueMessage);
		public abstract string GenerateSetStatusSucceededWithIssuesMessage(string statusMessage);
		public abstract string GenerateSetStatusFailMessage(string statusMessage);
	}
}
