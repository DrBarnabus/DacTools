// Copyright (c) 2019 DrBarnabus

namespace DacTools.Deployment.Core.Common
{
    public interface IBuildServerResolver
    {
        IBuildServer Resolve();
    }
}
