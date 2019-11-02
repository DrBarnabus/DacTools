// Copyright (c) 2019 DrBarnabus

// Copyright (c) 2019 DrBarnabus

// Copyright (c) 2019 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;

namespace DacTools.Deployment
{
    public interface IDeploymentExecutor
    {
        Task<int> Execute(Arguments arguments, CancellationToken cancellationToken);
    }
}
