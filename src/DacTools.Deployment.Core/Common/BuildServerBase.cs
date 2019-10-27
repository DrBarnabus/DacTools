// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.Common
{
    public abstract class BuildServerBase : IBuildServer
    {
        protected readonly ILog Log;

        protected BuildServerBase(IEnvironment environment, ILog log)
        {
            Log = log;
            Environment = environment;
        }

        protected IEnvironment Environment { get; }

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
