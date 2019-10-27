using System.Threading.Tasks;

namespace DacTools.Deployment.Core
{
    public interface IDacPacDeployer
    {
        Task DeployDacPac();
    }
}
