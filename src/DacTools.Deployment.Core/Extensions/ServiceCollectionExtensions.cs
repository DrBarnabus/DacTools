// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DacTools.Deployment.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServiceModule<TServiceModule>(this IServiceCollection serviceCollection)
            where TServiceModule : IServiceModule, new()
        {
            var serviceModule = new TServiceModule();
            serviceModule.RegisterTypes(serviceCollection);
        }
    }
}
