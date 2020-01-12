// Copyright (c) 2020 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.DatabaseListGenerators;

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

        public void Setup(DatabaseInfo databaseInfo, AsyncTaskBase.ProgressUpdateDelegate progressUpdate)
        {
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            _asyncTaskCompleted(this);
        }
    }
}
