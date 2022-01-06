// Copyright (c) 2022 DrBarnabus

using Cake.Frosting;
using Common;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Publish
{
    public class Startup : IFrostingStartup
    {
        public void Configure(IServiceCollection services)
        {
            services.UseLifetime<BuildLifetime>();
            services.UseTaskLifetime<DefaultTaskLifetime>();

            services.UseWorkingDirectory(DirectoryPathExtensions.GetRootDirectory());

            services.UseTool(new Uri("dotnet:?package=GitVersion.Tool&version=5.7.0"));
        }
    }
}
