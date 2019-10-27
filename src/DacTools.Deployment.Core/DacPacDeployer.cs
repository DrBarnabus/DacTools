// Copyright (c) 2019 DrBarnabus

using System.Threading.Tasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core
{
    internal class DacPacDeployer : IDacPacDeployer
    {
        private readonly IBuildServerResolver _buildServerResolver;
        private readonly ILog _log;

        public DacPacDeployer(ILog log, IBuildServerResolver buildServerResolver)
        {
            _log = log;
            _buildServerResolver = buildServerResolver;
        }

        public Task DeployDacPac()
        {
            var buildServer = _buildServerResolver.Resolve();

            return Task.CompletedTask;
        }
    }
}
