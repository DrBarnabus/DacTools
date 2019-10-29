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
            try
            {
                await CreateHostBuilder(args).Build().RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("A fatal error occurred: " + ex.Message);
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
