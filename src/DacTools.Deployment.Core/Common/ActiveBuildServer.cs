// Copyright (c) 2022 DrBarnabus

using System;
using System.Collections.Generic;
using System.Threading;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.Common;

public class ActiveBuildServer : IActiveBuildServer
{
    private readonly IEnumerable<IBuildServer> _buildServers;
    private readonly ILog _log;

    private int _isResolved;
    private IBuildServer? _resolvedBuildServer;

    public ActiveBuildServer(IEnumerable<IBuildServer> buildServers, ILog log)
    {
        _buildServers = buildServers;
        _log = log;
    }

    public bool IsActive => Instance != null;

    public IBuildServer? Instance
    {
        get
        {
            if (Interlocked.Exchange(ref _isResolved, 1) == 1)
                return _resolvedBuildServer;

            _resolvedBuildServer = ResolveBuildServer();
            return _resolvedBuildServer;
        }
    }

    private IBuildServer? ResolveBuildServer()
    {
        foreach (var buildServer in _buildServers)
            try
            {
                _log.Debug($"Checking if build server '{buildServer.GetType().Name}' is applicable.");
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
