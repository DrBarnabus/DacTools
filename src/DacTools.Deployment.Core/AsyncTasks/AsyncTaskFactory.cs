// Copyright (c) 2020 DrBarnabus

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

        public AsyncTaskFactory(IOptions<Arguments> arguments, ILog log)
        {
            _arguments = arguments.Value;
            _log = log;
        }

        public TAsyncTask CreateAsyncTask() => (TAsyncTask)Activator.CreateInstance(typeof(TAsyncTask), _arguments, _log);
    }
}
