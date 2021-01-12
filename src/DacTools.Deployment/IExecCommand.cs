// Copyright (c) 2021 DrBarnabus

using System.Threading;
using System.Threading.Tasks;

namespace DacTools.Deployment
{
    public interface IExecCommand
    {
        Task Execute(CancellationToken cancellationToken);
    }
}
