// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment.Core.BuildServers
{
    public class AzurePipelines : BuildServerBase
    {
        private readonly Arguments _arguments;

        public AzurePipelines(IEnvironment environment, ILog log, IOptions<Arguments> arguments) : base(environment, log)
        {
            _arguments = arguments.Value;
        }

        protected override string EnvironmentVariable { get; } = "TF_BUILD";

        public override bool CanApplyToCurrentContext()
        {
            return base.CanApplyToCurrentContext() || _arguments.AzPipelines;
        }

        public override string GenerateSetProgressMessage(int current, int total, string message) =>
            $"##vso[task.setprogress value={(int)(current / (double)total * 100.0f)};] {message}";

        public override string GenerateLogIssueWarningMessage(string issueMessage) =>
            $"##vso[task.logissue type=warning;] {issueMessage}";

        public override string GenerateLogIssueErrorMessage(string issueMessage) =>
            $"##vso[task.logissue type=error;] {issueMessage}";

        public override string GenerateSetStatusSucceededMessage(string statusMessage) =>
            $"##vso[task.complete result=Succeeded;] {statusMessage}";

        public override string GenerateSetStatusSucceededWithIssuesMessage(string statusMessage) =>
            $"##vso[task.complete result=SucceededWithIssues;] {statusMessage}";

        public override string GenerateSetStatusFailMessage(string statusMessage) =>
            $"##vso[task.complete result=Failed;] {statusMessage}";
    }
}
