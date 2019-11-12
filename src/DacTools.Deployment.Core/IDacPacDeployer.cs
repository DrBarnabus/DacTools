// Copyright (c) 2019 DrBarnabus

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.DatabaseListGenerators;

namespace DacTools.Deployment.Core
{
    public interface IDacPacDeployer
    {
        Task DeployDacPac(IReadOnlyCollection<DatabaseInfo> databases, CancellationToken cancellationToken);
    }
}
