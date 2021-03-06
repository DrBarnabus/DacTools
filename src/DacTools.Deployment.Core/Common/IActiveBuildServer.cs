﻿// Copyright (c) 2021 DrBarnabus

namespace DacTools.Deployment.Core.Common
{
    public interface IActiveBuildServer
    {
        bool IsActive { get; }

        IBuildServer Instance { get; }
    }
}
