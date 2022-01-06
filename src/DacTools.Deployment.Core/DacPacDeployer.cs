// Copyright (c) 2022 DrBarnabus

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment.Core;

internal class DacPacDeployer : IDacPacDeployer
{
    private readonly Arguments _arguments;
    private readonly IActiveBuildServer _buildServer;
    private readonly IAsyncTaskFactory<DacPacDeployAsyncTask> _dacPacDeployAsyncTaskFactory;
    private readonly ILog _log;

    public DacPacDeployer(ILog log, IActiveBuildServer buildServer, IOptions<Arguments> arguments,
        IAsyncTaskFactory<DacPacDeployAsyncTask> dacPacDeployAsyncTaskFactory)
    {
        _log = log;
        _buildServer = buildServer;
        _dacPacDeployAsyncTaskFactory = dacPacDeployAsyncTaskFactory;
        _arguments = arguments.Value;
    }

    public async Task DeployDacPac(IReadOnlyCollection<DatabaseInfo> databases, CancellationToken cancellationToken)
    {
        _log.Debug("Starting DacPac Deployment Tasks with {0} {1}.", _arguments.Threads,
            _arguments.Threads == 1 ? "thread" : "threads");

        int completedTasks = 0;
        int failedTasks = 0;
        int totalTasks = databases.Count;

        var asyncTaskRunner = new AsyncTaskRunner<DacPacDeployAsyncTask>(_arguments.Threads, cancellationToken);
        foreach (var database in databases)
        {
            var dacPacDeployAsyncTask = _dacPacDeployAsyncTaskFactory.CreateAsyncTask();
            dacPacDeployAsyncTask.Setup(database, (_, successful, _) =>
            {
                Interlocked.Increment(ref completedTasks);
                _log.Info("Completed {0} out of {1} tasks.", completedTasks, totalTasks);

                if (!successful)
                {
                    Interlocked.Increment(ref failedTasks);
                    _log.Warning("{0} out of {1} tasks have failed.", failedTasks, totalTasks);
                }

                if (_buildServer.IsActive && _buildServer.Instance is not null)
                    _log.WriteRaw(LogLevel.Info,
                        _buildServer.Instance.GenerateSetProgressMessage(completedTasks, totalTasks,
                            "Progress Update"));
            });

            asyncTaskRunner.AddTask(dacPacDeployAsyncTask);
        }

        await asyncTaskRunner.WaitForCompletion();

        if (failedTasks == totalTasks) // If all Tasks Failed
        {
            _log.Error("All Deployment Tasks Failed");
            if (_buildServer.IsActive && _buildServer.Instance is not null)
                _log.WriteRaw(LogLevel.Error,
                    _buildServer.Instance.GenerateSetStatusFailMessage("All Deployment Tasks Failed"));
        }
        else if (failedTasks > 0) // If any Tasks Failed
        {
            _log.Warning("Some Deployment Tasks Failed");
            if (_buildServer.IsActive && _buildServer.Instance is not null)
                _log.WriteRaw(LogLevel.Warn,
                    _buildServer.Instance.GenerateSetStatusSucceededWithIssuesMessage("Some Deployment Tasks Failed"));
        }
        else // If all Tasks Succeeded
        {
            _log.Info("All Deployment Tasks Succeeded");
            if (_buildServer.IsActive && _buildServer.Instance is not null)
                _log.WriteRaw(LogLevel.Info,
                    _buildServer.Instance.GenerateSetStatusSucceededMessage("All Deployment Tasks Succeeded"));
        }
    }
}
