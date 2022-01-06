// Copyright (c) 2022 DrBarnabus

namespace DacTools.Deployment.Core.Common
{
    public interface IEnvironment
    {
        string? GetEnvironmentVariable(string variableName);

        void SetEnvironmentVariable(string variableName, string value);
    }
}
