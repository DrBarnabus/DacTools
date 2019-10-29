// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core
{
    public class Arguments
    {
        public bool IsVersion;
        public bool IsHelp;

        public string DacPacFilePath;

        public LogLevel LogLevel = LogLevel.Info;
    }
}
