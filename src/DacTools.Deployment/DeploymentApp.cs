// Copyright (c) 2020 DrBarnabus

using System;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Exceptions;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DeploymentApp : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly Arguments _arguments;
        private readonly IDeploymentExecutor _deploymentExecutor;
        private readonly ILog _log;

        public DeploymentApp(IHostApplicationLifetime applicationLifetime, IDeploymentExecutor deploymentExecutor, ILog log, IOptions<Arguments> options)
        {
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _deploymentExecutor = deploymentExecutor ?? throw new ArgumentNullException(nameof(deploymentExecutor));
            _log = log;
            _arguments = options.Value;

            log.LogLevel = _arguments.LogLevel;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _deploymentExecutor.Execute(_arguments, cancellationToken);
            }
            catch (FatalException fatalException)
            {
                if (fatalException.ShouldLog)
                    _log.Error(fatalException.Message);

                throw;
            }
            catch (Exception ex)
            {
                _log.Error("An unexpected error ocurred: {0}", ex.Message);
                throw new FatalException("An unexpected error occurred.", ex);
            }

            _applicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
