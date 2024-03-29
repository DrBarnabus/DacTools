﻿// Copyright (c) 2022 DrBarnabus

namespace DacTools.Deployment.Core.Common;

public class Environment : IEnvironment
{
    public string? GetEnvironmentVariable(string variableName)
    {
        return System.Environment.GetEnvironmentVariable(variableName);
    }

    public void SetEnvironmentVariable(string variableName, string value)
    {
        System.Environment.SetEnvironmentVariable(variableName, value);
    }
}
