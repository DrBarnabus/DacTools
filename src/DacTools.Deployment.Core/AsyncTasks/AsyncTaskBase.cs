// Copyright (c) 2021 DrBarnabus

using DacTools.Deployment.Core.Common;
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
        private readonly IActiveBuildServer _buildServer;

        protected AsyncTaskBase(Arguments arguments, ILog log, IActiveBuildServer buildServer)
        {
            Arguments = arguments;
            _log = log;
            _buildServer = buildServer;
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

            if (_buildServer != null)
                _log.WriteRaw(LogLevel.Error, _buildServer.GenerateLogIssueErrorMessage(string.Format($"'{DatabaseInfo}' {source} - {formatString}", args)));
        }

        protected void LogWarning(string source, string formatString, params object[] args)
        {
            _log.Warning($"'{DatabaseInfo}' {source} - {formatString}", args);

            if (_buildServer != null)
                _log.WriteRaw(LogLevel.Warn, _buildServer.GenerateLogIssueWarningMessage(string.Format($"'{DatabaseInfo}' {source} - {formatString}", args)));
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
