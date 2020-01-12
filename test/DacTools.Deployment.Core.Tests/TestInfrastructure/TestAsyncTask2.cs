// Copyright (c) 2019 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.Tests.TestInfrastructure
{
    public class TestAsyncTask2 : AsyncTaskBase
    {
        public TestAsyncTask2(Arguments arguments, ILog log) : base(arguments, log)
        {
        }

        public Arguments PublicArguments => Arguments;

        public ProgressUpdateDelegate PublicProgressUpdate => ProgressUpdate;

        public override Task Run(CancellationToken cancellationToken)
        {
            LogDebug("Internal", "Test");
            LogInfo("Internal", "Test");
            LogWarning("Internal", "Test");
            LogError("Internal", "Test");

            ProgressUpdate(this, true, 100);

            return Task.CompletedTask;
        }
    }
}