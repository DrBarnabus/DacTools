// Copyright (c) 2021 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public class AsyncTaskRunner<TAsyncTask> where TAsyncTask : IAsyncTask
    {
        public AsyncTaskRunner(int maxDegreeOfParallelism, CancellationToken cancellationToken)
        {
            ActionBlock = new ActionBlock<TAsyncTask>(async at => await at.Run(cancellationToken), new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = cancellationToken
            });
        }

        public ActionBlock<TAsyncTask> ActionBlock { get; }

        public void AddTask(TAsyncTask asyncTask) => ActionBlock.Post(asyncTask);

        public async Task WaitForCompletion()
        {
            ActionBlock.Complete();
            await ActionBlock.Completion;
        }
    }
}
