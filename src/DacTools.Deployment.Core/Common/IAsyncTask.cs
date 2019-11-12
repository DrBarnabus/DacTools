// Copyright (c) 2019 DrBarnabus

using System.Threading;

namespace DacTools.Deployment.Core.Common
{
    public interface IAsyncTask
    {
        void RunAsync(CancellationToken cancellationToken);
    }
}
