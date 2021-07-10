// Copyright (c) 2021 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.DatabaseListGenerators;
using System;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public interface IAsyncTask
    {
        DatabaseInfo DatabaseInfo { get; }

        void Setup(DatabaseInfo databaseInfo, Action<IAsyncTask, bool, long>  progressUpdate);
        Task Run(CancellationToken cancellationToken);
    }
}
