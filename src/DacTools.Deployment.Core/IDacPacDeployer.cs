// Copyright (c) 2019 DrBarnabus

using System.Threading.Tasks;

namespace DacTools.Deployment.Core
{
    public interface IDacPacDeployer
    {
        Task DeployDacPac();
    }
}
