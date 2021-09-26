// Copyright (c) 2021 DrBarnabus

using Cake.Frosting;
using Common;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Xml.Linq;

namespace Build
{
    public class Startup : IFrostingStartup
    {
        public void Configure(IServiceCollection services)
        {
            services.UseLifetime<BuildLifetime>();
            services.UseTaskLifetime<DefaultTaskLifetime>();

            services.UseWorkingDirectory(DirectoryPathExtensions.GetRootDirectory());

            services.UseTool(new Uri("dotnet:?package=Codecov.Tool&version=1.13.0"));
            services.UseTool(new Uri("dotnet:?package=GitVersion.Tool&version=5.7.0"));
        }
    }
}
