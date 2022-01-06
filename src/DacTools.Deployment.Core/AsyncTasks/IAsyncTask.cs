// Copyright (c) 2022 DrBarnabus

using System;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.DatabaseListGenerators;

namespace DacTools.Deployment.Core.AsyncTasks;

public interface IAsyncTask
{
    DatabaseInfo? DatabaseInfo { get; }

    void Setup(DatabaseInfo databaseInfo, Action<IAsyncTask, bool, long> progressUpdate);

    Task Run(CancellationToken cancellationToken);
}
