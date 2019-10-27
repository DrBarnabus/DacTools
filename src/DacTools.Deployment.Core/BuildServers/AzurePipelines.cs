// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.BuildServers
{
    public class AzurePipelines : BuildServerBase
    {
        public AzurePipelines(IEnvironment environment, ILog log) : base(environment, log)
        {
        }

        protected override string EnvironmentVariable { get; } = "TF_BUILD";

        public override string GenerateSetProgressMessage(int current, int total, string message) =>
            $"##vso[task.setprogress value={(int)(current / (double)total * 100.0f)};] {message}";

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
