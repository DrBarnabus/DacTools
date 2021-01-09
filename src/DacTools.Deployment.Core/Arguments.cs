// Copyright (c) 2020 DrBarnabus

using System.Collections.Generic;
using DacTools.Deployment.Core.Logging;
using Microsoft.SqlServer.Dac;

namespace DacTools.Deployment.Core
{
    public class Arguments
    {
        public readonly ISet<string> DatabaseNames = new HashSet<string>();
        public DacDeployOptions DacDeployOptions;

        public string DacPacFilePath;
        public bool IsBlacklist;
        public bool IsHelp;
        public bool IsVersion;
        public LogLevel LogLevel = LogLevel.Info;
        public string LogFilePath = null;
        public string MasterConnectionString;
        public int Threads;

        public Arguments()
        {
            SetDefaultDacDeployOptions();
        }

        public void AddDatabaseName(string databaseName) => DatabaseNames.Add(databaseName);

        public void SetDefaultDacDeployOptions()
        {
            DacDeployOptions = new DacDeployOptions
            {
                BlockOnPossibleDataLoss = false,
                DropIndexesNotInSource = false,
                IgnorePermissions = true,
                IgnoreRoleMembership = true,
                GenerateSmartDefaults = true,
                DropObjectsNotInSource = true,
                DoNotDropObjectTypes = new[]
                {
                    ObjectType.Logins,
                    ObjectType.Users,
                    ObjectType.Permissions,
                    ObjectType.RoleMembership,
                    ObjectType.Filegroups
                },
                CommandTimeout = 0 // Infinite Timeout
            };
        }
    }
}
