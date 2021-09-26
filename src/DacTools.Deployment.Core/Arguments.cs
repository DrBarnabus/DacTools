// Copyright (c) 2021 DrBarnabus

using System.Collections.Generic;
using DacTools.Deployment.Core.Logging;
using Microsoft.SqlServer.Dac;

namespace DacTools.Deployment.Core
{
    public class Arguments
    {
        public readonly ISet<string> DatabaseNames = new HashSet<string>();
        public DacDeployOptions DacDeployOptions;

        public string? DacPacFilePath;
        public bool IsBlacklist;
        public bool IsHelp;
        public bool IsVersion;
        public LogLevel LogLevel = LogLevel.Info;
        public string? LogFilePath = null;
        public string? MasterConnectionString;
        public int Threads;

        public bool AzPipelines;

        public Arguments()
        {
            DacDeployOptions = new DacDeployOptions();
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
                IgnoreFileAndLogFilePath = true,
                GenerateSmartDefaults = true,
                DropObjectsNotInSource = true,
                DoNotDropObjectTypes = new[]
                {
                    ObjectType.Filegroups,
                    ObjectType.Files,
                    ObjectType.Logins,
                    ObjectType.Permissions,
                    ObjectType.RoleMembership,
                    ObjectType.Users
                },
                CommandTimeout = 0 // Infinite Timeout
            };
        }
    }
}
