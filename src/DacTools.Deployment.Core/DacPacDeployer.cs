using System.Threading.Tasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core
{
    class DacPacDeployer : IDacPacDeployer
    {
        private readonly ILog _log;
        private readonly IBuildServerResolver _buildServerResolver;

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
