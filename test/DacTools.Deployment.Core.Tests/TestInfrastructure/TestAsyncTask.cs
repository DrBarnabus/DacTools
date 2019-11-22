// Copyright (c) 2019 DrBarnabus

using System.Threading;
using DacTools.Deployment.Core.AsyncTasks;

namespace DacTools.Deployment.Core.Tests.TestInfrastructure
{
    public class TestAsyncTask : IAsyncTask
    {
        public delegate void AsyncTaskCompletedDelegate(TestAsyncTask task);

        private readonly AsyncTaskCompletedDelegate _asyncTaskCompleted;

        public TestAsyncTask(int taskId, AsyncTaskCompletedDelegate asyncTaskCompleted)
        {
            TaskId = taskId;
            _asyncTaskCompleted = asyncTaskCompleted;
        }

        public int TaskId { get; }

        public void RunAsync(CancellationToken cancellationToken)
        {
            _asyncTaskCompleted(this);
        }
    }
}
