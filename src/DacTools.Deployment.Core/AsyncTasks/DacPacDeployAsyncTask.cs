// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.Logging;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;

namespace DacTools.Deployment.Core.AsyncTasks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DacPacDeployAsyncTask : AsyncTaskBase
    {
        public DacPacDeployAsyncTask(Arguments arguments, ILog log, IActiveBuildServer buildServer)
            : base(arguments, log, buildServer)
        {
        }

        public override Task Run(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (DatabaseInfo is null)
                throw new InvalidOperationException("DatabaseInfo was null unable to process task.");

            var stopwatch = Stopwatch.StartNew();

            try
            {
                LogDebug("Internal", "DacPac Deployment AsyncTask Started.");

                var dacPackage = DacPackage.Load(Arguments.DacPacFilePath);

                var dacServices = new DacServices(GetConnectionString(DatabaseInfo.Name));

                dacServices.Message += (_, args) =>
                {
                    switch (args.Message.MessageType)
                    {
                        case DacMessageType.Error:
                            LogError("DacServices", args.Message.ToString());
                            break;
                        case DacMessageType.Warning:
                            LogWarning("DacServices", args.Message.ToString());
                            break;
                        case DacMessageType.Message:
                            LogInfo("DacServices", args.Message.ToString());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                };

                dacServices.ProgressChanged += (_, args) => LogDebug("DacServices Progress", "OperationId {0}, Status {1}, Message {2}", args.OperationId, args.Status, args.Message);

                dacServices.Deploy(dacPackage, DatabaseInfo.Name, true, Arguments.DacDeployOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                LogError("Internal", "DacPac Deployment AsyncTask failed after {0}ms with the following error: {1}\n{2}", stopwatch.ElapsedMilliseconds, ex.Message, ex.StackTrace);

                ProgressUpdate?.Invoke(this, false, stopwatch.ElapsedMilliseconds);
                return Task.CompletedTask;
            }

            LogInfo("Internal", "DacPac Deployment AsyncTask completed successfully in {0}ms.", stopwatch.ElapsedMilliseconds);

            ProgressUpdate?.Invoke(this, true, stopwatch.ElapsedMilliseconds);
            return Task.CompletedTask;
        }

        private string GetConnectionString(string databaseName)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(Arguments.MasterConnectionString)
            {
                InitialCatalog = databaseName
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }
    }
}
