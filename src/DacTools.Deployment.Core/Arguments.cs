// Copyright (c) 2019 DrBarnabus

using System.Collections.Generic;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core
{
    public class Arguments
    {
        public readonly ISet<string> DatabaseNames = new HashSet<string>();

        public string DacPacFilePath;
        public bool IsBlacklist;
        public bool IsHelp;
        public bool IsVersion;

        public LogLevel LogLevel = LogLevel.Info;
        public string MasterConnectionString;
        public int Threads;
        public void AddDatabaseName(string databaseName) => DatabaseNames.Add(databaseName);
    }
}
