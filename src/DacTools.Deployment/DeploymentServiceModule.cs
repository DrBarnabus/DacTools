// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DacTools.Deployment;

public class DeploymentServiceModule : IServiceModule
{
    public void RegisterTypes(IServiceCollection services)
    {
        services.AddSingleton<IArgumentParser, ArgumentParser>();
        services.AddSingleton<IHelpWriter, HelpWriter>();
        services.AddSingleton<IVersionWriter, VersionWriter>();
        services.AddSingleton<IDeploymentExecutor, DeploymentExecutor>();
        services.AddSingleton<IExecCommand, ExecCommand>();
    }
}
