// Copyright (c) 2019 DrBarnabus

using System;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public abstract class AsyncTaskBase : IAsyncTask
    {
        public delegate void ProgressUpdateDelegate(AsyncTaskBase asyncTask, bool successful, long elapsedMilliseconds);

        private readonly ILog _log;

        protected AsyncTaskBase(Arguments arguments, ILog log)
        {
            Arguments = arguments;
            _log = log;
        }

        protected Arguments Arguments { get; }
        protected ProgressUpdateDelegate ProgressUpdate { get; private set; }

        public DatabaseInfo DatabaseInfo { get; private set; }

        public void Setup(DatabaseInfo databaseInfo, ProgressUpdateDelegate progressUpdate)
        {
            DatabaseInfo = databaseInfo ?? throw new ArgumentNullException(nameof(databaseInfo));
            ProgressUpdate = progressUpdate ?? throw new ArgumentNullException(nameof(progressUpdate));
        }

        public abstract Task Run(CancellationToken cancellationToken);

        protected void LogError(string source, string formatString, params object[] args)
        {
            _log.Error($"'{DatabaseInfo}' {source} - {formatString}", args);
        }

        protected void LogWarning(string source, string formatString, params object[] args)
        {
            _log.Warning($"'{DatabaseInfo}' {source} - {formatString}", args);
        }

        protected void LogInfo(string source, string formatString, params object[] args)
        {
            _log.Info($"'{DatabaseInfo}' {source} - {formatString}", args);
        }

        protected void LogDebug(string source, string formatString, params object[] args)
        {
            _log.Debug($"'{DatabaseInfo}' {source} - {formatString}", args);
        }
    }
}
