// Copyright (c) 2021 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.DatabaseListGenerators;
using System;

namespace DacTools.Deployment.Core.Tests.TestInfrastructure
{
    public class TestAsyncTask : IAsyncTask
    {
        private readonly Action<TestAsyncTask> _asyncTaskCompleted;

        public TestAsyncTask(int taskId, Action<TestAsyncTask> asyncTaskCompleted)
        {
            TaskId = taskId;
            _asyncTaskCompleted = asyncTaskCompleted;
        }

        public int TaskId { get; }

        public DatabaseInfo DatabaseInfo { get; } = null;

        public void Setup(DatabaseInfo databaseInfo, Action<IAsyncTask, bool, long> progressUpdate)
        {
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            await Task.Delay(10, cancellationToken);
            _asyncTaskCompleted(this);
        }
    }
}
