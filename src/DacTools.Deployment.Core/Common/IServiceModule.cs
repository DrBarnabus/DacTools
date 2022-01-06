// Copyright (c) 2022 DrBarnabus

using Microsoft.Extensions.DependencyInjection;

namespace DacTools.Deployment.Core.Common
{
    public interface IServiceModule
    {
        void RegisterTypes(IServiceCollection services);
    }
}
