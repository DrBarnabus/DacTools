// Copyright (c) 2021 DrBarnabus

namespace DacTools.Deployment.Core.AsyncTasks
{
    public interface IAsyncTaskFactory<out TAsyncTask>
        where TAsyncTask : IAsyncTask
    {
        TAsyncTask CreateAsyncTask();
    }
}
