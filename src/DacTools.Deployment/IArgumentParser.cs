// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core;

namespace DacTools.Deployment
{
    public interface IArgumentParser
    {
        Arguments ParseArguments(string commandLineArguments);
        Arguments ParseArguments(string[] commandLineArguments);
    }
}
