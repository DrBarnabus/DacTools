// Copyright (c) 2019 DrBarnabus

using System.Collections.Generic;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core
{
    public class Arguments
    {
        public bool IsVersion;
        public bool IsHelp;

        public string DacPacFilePath;
        public string MasterConnectionString;
        public bool IsBlacklist;
        public int Threads;

        public readonly ISet<string> DatabaseNames  = new HashSet<string>();
        public void AddDatabaseName(string databaseName) => DatabaseNames.Add(databaseName);

        public LogLevel LogLevel = LogLevel.Info;
    }
}
