// Copyright (c) 2019 DrBarnabus

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment.Core
{
    internal class DacPacDeployer : IDacPacDeployer
    {
        private readonly IBuildServerResolver _buildServerResolver;
        private readonly ILog _log;
        private readonly Arguments _arguments;

        private readonly object _lock = new object();

        public DacPacDeployer(ILog log, IBuildServerResolver buildServerResolver, IOptions<Arguments> arguments)
        {
            _log = log;
            _buildServerResolver = buildServerResolver;
            _arguments = arguments.Value;
        }

        public async Task DeployDacPac(IReadOnlyCollection<DatabaseInfo> databases, CancellationToken cancellationToken)
        {
            var buildServer = _buildServerResolver.Resolve();

            _log.Debug("Starting DacPac Deployment Tasks with {0} {1}.", _arguments.Threads, _arguments.Threads == 1 ? "thread" : "threads");

            int completedTasks = 0;
            int totalTasks = databases.Count;
            var asyncTaskRunner = new AsyncTaskRunner<DacDeployAsyncTask>(_arguments.Threads, cancellationToken);
            foreach (var database in databases)
            {
                var dacDeployAsyncTask = new DacDeployAsyncTask(_log, database, result =>
                {
                    lock (_lock)
                    {
                        completedTasks++;
                        buildServer?.GenerateSetProgressMessage(completedTasks, totalTasks, "DacPac Deployment Progress");
                    }
                });

                asyncTaskRunner.AddTask(dacDeployAsyncTask);
            }

            await asyncTaskRunner.WaitForCompletion();
        }
    }
}
