// Copyright (c) 2021 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;
using System;

namespace DacTools.Deployment.Core.Tests.TestInfrastructure
{
    public class TestAsyncTask2 : AsyncTaskBase
    {
        public TestAsyncTask2(Arguments arguments, ILog log, IActiveBuildServer buildServer) : base(arguments, log, buildServer)
        {
        }

        public Arguments PublicArguments => Arguments;

        public Action<IAsyncTask, bool, long>? PublicProgressUpdate => ProgressUpdate;

        public override Task Run(CancellationToken cancellationToken)
        {
            LogDebug("Internal", "Test");
            LogInfo("Internal", "Test");
            LogWarning("Internal", "Test");
            LogError("Internal", "Test");

            ProgressUpdate?.Invoke(this, true, 100);

            return Task.CompletedTask;
        }
    }
}
