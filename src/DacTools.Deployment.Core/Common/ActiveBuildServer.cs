// Copyright (c) 2021 DrBarnabus

namespace DacTools.Deployment.Core.Common
{
    public class ActiveBuildServer : IActiveBuildServer
    {
        public bool IsActive { get; }

        public IBuildServer Instance { get; }

        public ActiveBuildServer(IBuildServer buildServer)
        {
            Instance = buildServer;
            IsActive = Instance != null;
        }
    }
}
