// Copyright (c) 2019 DrBarnabus

using System;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DacTools.Deployment
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) => { configApp.AddCommandLine(args); })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddServiceModule(new DeploymentCoreServiceModule());

                    services.AddHostedService<DeploymentApp>();
                })
                .UseConsoleLifetime();
    }
}
