﻿// Copyright (c) 2021 DrBarnabus

using Cake.Frosting;
using Common;
using Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Release
{
    public class Startup : IFrostingStartup
    {
        public void Configure(IServiceCollection services)
        {
            services.UseLifetime<BuildLifetime>();
            services.UseTaskLifetime<DefaultTaskLifetime>();

            services.UseWorkingDirectory(DirectoryPathExtensions.GetRootDirectory());

            services.UseTool(new Uri("dotnet:?package=GitVersion.Tool&version=5.7.0"));
            services.UseTool(new Uri("dotnet:?package=GitReleaseManager.Tool&version=0.12.1"));
        }
    }
}
