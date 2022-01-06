// Copyright (c) 2022 DrBarnabus

namespace DacTools.Deployment.Core.Common;

public interface IActiveBuildServer
{
    bool IsActive { get; }

    IBuildServer? Instance { get; }
}
