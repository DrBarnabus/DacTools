// Copyright (c) 2020 DrBarnabus

using System;
using System.Collections.Generic;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.Common
{
    public class BuildServerResolver : IBuildServerResolver
    {
        private readonly IEnumerable<IBuildServer> _buildServers;
        private readonly ILog _log;

        public BuildServerResolver(IEnumerable<IBuildServer> buildServers, ILog log)
        {
            _buildServers = buildServers;
            _log = log;
        }

        public IBuildServer Resolve()
        {
            foreach (var buildServer in _buildServers)
                try
                {
                    if (buildServer.CanApplyToCurrentContext())
                    {
                        _log.Info($"Applicable build server found: '{buildServer.GetType().Name}'.");
                        return buildServer;
                    }
                }
                catch (Exception ex)
                {
                    _log.Warning($"Failed to check build server '{buildServer.GetType().Name}': {ex.Message}");
                }

            return null;
        }
    }
}
