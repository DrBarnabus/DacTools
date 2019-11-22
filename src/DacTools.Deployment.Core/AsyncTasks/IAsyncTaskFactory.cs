// Copyright (c) 2019 DrBarnabus

namespace DacTools.Deployment.Core.AsyncTasks
{
    public interface IAsyncTaskFactory<out TAsyncTask>
        where TAsyncTask : IAsyncTask
    {
        TAsyncTask CreateAsyncTask();
    }
}
