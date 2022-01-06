// Copyright (c) 2022 DrBarnabus

using System;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Exceptions;
using DacTools.Deployment.Core.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            await CreateHostBuilder(args).Build().RunAsync();
            return 0;
        }
        catch (ArgumentParsingException ex)
        {
            Console.Error.WriteLine($"Failed to Parse Arguments with Error: {ex.Message}");
            Console.Error.WriteLine("For Usage Help, run the program with one of the following arguments; -?, -h or -help.");
            Console.Error.WriteLine("Alternatively, please consult the documentation for more details.");
            return 1;
        }
        catch (FatalException)
        {
            return 1;
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return new HostBuilder()
            .ConfigureAppConfiguration((_, configApp) => { configApp.AddCommandLine(args); })
            .ConfigureServices((_, services) =>
            {
                services.AddServiceModule<DeploymentCoreServiceModule>();
                services.AddServiceModule<DeploymentServiceModule>();

                services.AddSingleton(sp => Options.Create(sp.GetRequiredService<IArgumentParser>().ParseArguments(args)));

                services.AddHostedService<DeploymentApp>();
            })
            .UseConsoleLifetime();
    }
}
