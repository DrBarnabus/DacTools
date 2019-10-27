using System;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Hosting;

namespace DacTools.Deployment
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class DeploymentApp : IHostedService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;

        public DeploymentApp(IHostApplicationLifetime applicationLifetime, ILog log)
        {
            _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));

            log.LogLevel = LogLevel.Debug; // TODO: Get this from Arguments.
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                throw;
            }

            _applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
