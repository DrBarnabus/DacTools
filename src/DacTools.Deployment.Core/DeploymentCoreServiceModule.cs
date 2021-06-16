// Copyright (c) 2021 DrBarnabus

using DacTools.Deployment.Core.AsyncTasks;
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

            services.AddSingleton<IBuildServer, AzurePipelines>();
            services.AddSingleton<IBuildServerResolver, BuildServerResolver>();
            services.AddSingleton<IActiveBuildServer, ActiveBuildServer>(serviceProvider =>
            {
                var buildServerResolver = serviceProvider.GetRequiredService<IBuildServerResolver>();
                return new ActiveBuildServer(buildServerResolver.Resolve());
            });

            services.AddSingleton<IDacPacDeployer, DacPacDeployer>();

            RegisterDatabaseListGenerators(services);
            RegisterAsyncTaskFactories(services);
        }

        private static void RegisterDatabaseListGenerators(IServiceCollection services)
        {
            services.AddSingleton<IWhitelistDatabaseListGenerator, WhitelistDatabaseListGenerator>();
            services.AddSingleton<IBlacklistDatabaseListGenerator, BlacklistDatabaseListGenerator>();
        }

        private static void RegisterAsyncTaskFactories(IServiceCollection services)
        {
            services.AddSingleton<IAsyncTaskFactory<DacPacDeployAsyncTask>, AsyncTaskFactory<DacPacDeployAsyncTask>>();
        }
    }
}
