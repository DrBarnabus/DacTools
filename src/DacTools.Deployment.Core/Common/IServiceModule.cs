// Copyright (c) 2020 DrBarnabus

using Microsoft.Extensions.DependencyInjection;

namespace DacTools.Deployment.Core.Common
{
    public interface IServiceModule
    {
        void RegisterTypes(IServiceCollection services);
    }
}
