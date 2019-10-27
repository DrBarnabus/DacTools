using DacTools.Deployment.Core.BuildServers;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DacTools.Deployment.Core
{
    public class DeploymentCoreServiceModule : IServiceModule
    {
        public void RegisterTypes(IServiceCollection services)
        {
            services.AddSingleton<IEnvironment, Environment>();
            services.AddSingleton<ILog, Log>();

            services.AddSingleton<IBuildServerResolver, BuildServerResolver>();
            services.AddSingleton<IBuildServer, AzurePipelines>();

            services.AddSingleton<IDacPacDeployer, DacPacDeployer>();
        }
    }
}
