// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core.BuildServers;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.DatabaseListGenerators;
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

            RegisterDatabaseListGenerators(services);
        }

        private void RegisterDatabaseListGenerators(IServiceCollection services)
        {
            services.AddSingleton<IWhitelistDatabaseListGenerator, WhitelistDatabaseListGenerator>();
            services.AddSingleton<IBlacklistDatabaseListGenerator, BlacklistDatabaseListGenerator>();
        }
    }
}
