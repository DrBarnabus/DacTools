// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core.Common;
using System;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public class AsyncTaskFactory<TAsyncTask> : IAsyncTaskFactory<TAsyncTask>
        where TAsyncTask : AsyncTaskBase
    {
        private readonly Arguments _arguments;
        private readonly ILog _log;
        private readonly IActiveBuildServer _buildServer;

        public AsyncTaskFactory(IOptions<Arguments> arguments, ILog log, IActiveBuildServer buildServer)
        {
            _arguments = arguments.Value;
            _log = log;
            _buildServer = buildServer;
        }

        public TAsyncTask CreateAsyncTask() => (TAsyncTask)Activator.CreateInstance(typeof(TAsyncTask), _arguments, _log, _buildServer);
    }
}
