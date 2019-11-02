// Copyright (c) 2019 DrBarnabus

using System;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DeploymentApp : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IDeploymentExecutor _deploymentExecutor;
        private readonly Arguments _arguments;

        public DeploymentApp(IHostApplicationLifetime applicationLifetime, IDeploymentExecutor deploymentExecutor, IOptions<Arguments> options, ILog log)
        {
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _deploymentExecutor = deploymentExecutor ?? throw new ArgumentNullException(nameof(deploymentExecutor));
            _arguments = options.Value;

            log.LogLevel = _arguments.LogLevel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _deploymentExecutor.Execute(_arguments, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }

            _applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
