// Copyright (c) 2020 DrBarnabus

namespace DacTools.Deployment.Core.Common
{
    public interface IBuildServerResolver
    {
        IBuildServer Resolve();
    }
}
