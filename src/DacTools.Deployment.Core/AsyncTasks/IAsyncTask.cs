// Copyright (c) 2020 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.DatabaseListGenerators;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public interface IAsyncTask
    {
        void Setup(DatabaseInfo databaseInfo, AsyncTaskBase.ProgressUpdateDelegate progressUpdate);
        Task Run(CancellationToken cancellationToken);
    }
}
