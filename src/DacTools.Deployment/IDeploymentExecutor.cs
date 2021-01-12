// Copyright (c) 2021 DrBarnabus

using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;

namespace DacTools.Deployment
{
    public interface IDeploymentExecutor
    {
        Task Execute(Arguments arguments, CancellationToken cancellationToken);
    }
}
