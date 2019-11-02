// Copyright (c) 2019 DrBarnabus

using System;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // So that we can return exit code 1 (0x1) when there is an error, anything being run in the host can throw an exception
            // which should reach this try-catch and return the correct exit code.
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
                    services.AddServiceModule(new DeploymentServiceModule());

                    services.AddSingleton(sp => Options.Create(sp.GetService<IArgumentParser>().ParseArguments(args)));

                    services.AddHostedService<DeploymentApp>();
                })
                .UseConsoleLifetime();
    }
}
