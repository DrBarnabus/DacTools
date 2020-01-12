// Copyright (c) 2020 DrBarnabus

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.AsyncTasks
{
    public class AsyncTaskRunnerTests
    {
        private readonly object _lock = new object();

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 10)]
        [InlineData(4, 4)]
        [InlineData(4, 40)]
        public async Task ShouldRunAllAsyncTasks(int maxDegreeOfParallelism, int taskCount)
        {
            // Setup
            var testAsyncTaskStatuses = new Dictionary<int, bool>();
            var asyncTaskRunner = new AsyncTaskRunner<TestAsyncTask>(maxDegreeOfParallelism, CancellationToken.None);
            for (int i = 0; i < taskCount; i++)
            {
                testAsyncTaskStatuses.Add(i, false);
                asyncTaskRunner.AddTask(new TestAsyncTask(i, task =>
                {
                    lock (_lock)
                    {
                        testAsyncTaskStatuses[task.TaskId] = true;
                    }
                }));
            }

            // Act
            await asyncTaskRunner.WaitForCompletion();

            // Assert
            testAsyncTaskStatuses.ShouldAllBe(s => s.Value);
        }
    }
}
