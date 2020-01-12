// Copyright (c) 2020 DrBarnabus

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
        private readonly Arguments _arguments;
        private readonly IBuildServerResolver _buildServerResolver;
        private readonly IAsyncTaskFactory<DacPacDeployAsyncTask> _dacPacDeployAsyncTaskFactory;

        private readonly object _lock = new object();
        private readonly ILog _log;

        public DacPacDeployer(ILog log, IBuildServerResolver buildServerResolver, IOptions<Arguments> arguments, IAsyncTaskFactory<DacPacDeployAsyncTask> dacPacDeployAsyncTaskFactory)
        {
            _log = log;
            _buildServerResolver = buildServerResolver;
            _dacPacDeployAsyncTaskFactory = dacPacDeployAsyncTaskFactory;
            _arguments = arguments.Value;
        }

        public async Task DeployDacPac(IReadOnlyCollection<DatabaseInfo> databases, CancellationToken cancellationToken)
        {
            var buildServer = _buildServerResolver.Resolve();

            // ReSharper disable once InconsistentlySynchronizedField
            _log.Debug("Starting DacPac Deployment Tasks with {0} {1}.", _arguments.Threads, _arguments.Threads == 1 ? "thread" : "threads");

            int completedTasks = 0;
            int totalTasks = databases.Count;
            var asyncTaskRunner = new AsyncTaskRunner<DacPacDeployAsyncTask>(_arguments.Threads, cancellationToken);
            foreach (var database in databases)
            {
                var dacPacDeployAsyncTask = _dacPacDeployAsyncTaskFactory.CreateAsyncTask();
                dacPacDeployAsyncTask.Setup(database, (task, successful, elapsedMilliseconds) =>
                {
                    lock (_lock)
                    {
                        completedTasks++;
                        _log.Debug("Completed {0} out of {1} tasks.", completedTasks, totalTasks);
                        buildServer?.GenerateSetProgressMessage(completedTasks, totalTasks, "Progress Update");
                    }
                });

                asyncTaskRunner.AddTask(dacPacDeployAsyncTask);
            }

            await asyncTaskRunner.WaitForCompletion();
        }
    }
}
