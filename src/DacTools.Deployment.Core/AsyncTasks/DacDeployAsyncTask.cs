// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public class DacDeployAsyncTask : IAsyncTask
    {
        private readonly ILog _log;
        private readonly DatabaseInfo _databaseInfo;
        private readonly Action<bool> _progressUpdate;

        public DacDeployAsyncTask(ILog log, DatabaseInfo databaseInfo, Action<bool> progressUpdate)
        {
            _log = log;
            _databaseInfo = databaseInfo;
            _progressUpdate = progressUpdate;
        }

        public void RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var taskTimer = Stopwatch.StartNew();

            _log.Debug("DacPac Deployment Task Started for '{0}'.", _databaseInfo.ToString());

            _progressUpdate(true);
            _log.Debug("DacPac Deployment Task for '{0}' was completed successfully in {1}ms.", _databaseInfo.ToString(), taskTimer.ElapsedMilliseconds);
        }
    }
}
