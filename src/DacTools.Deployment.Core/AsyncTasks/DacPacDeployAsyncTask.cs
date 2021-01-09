// Copyright (c) 2020 DrBarnabus

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
        public DacPacDeployAsyncTask(Arguments arguments, ILog log, IBuildServerResolver buildServerResolver)
            : base(arguments, log, buildServerResolver)
        {
        }

        public override Task Run(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();

            try
            {
                LogDebug("Internal", "DacPac Deployment AsyncTask Started.");

                var dacPackage = DacPackage.Load(Arguments.DacPacFilePath);

                var dacServices = new DacServices(GetConnectionString());

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

                dacServices.ProgressChanged += (_, args) => LogInfo("DacServices Progress", "OperationId {0}, Status {1}, Message {2}", args.OperationId, args.Status, args.Message);

                dacServices.Deploy(dacPackage, DatabaseInfo.Name, true, Arguments.DacDeployOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                LogError("Internal", "DacPac Deployment AsyncTask failed after {0}ms with the following error: {1}", stopwatch.ElapsedMilliseconds, ex.Message);

                ProgressUpdate(this, false, stopwatch.ElapsedMilliseconds);
                return Task.CompletedTask;
            }

            LogInfo("Internal", "DacPac Deployment AsyncTask completed successfully in {0}ms.", stopwatch.ElapsedMilliseconds);

            ProgressUpdate(this, true, stopwatch.ElapsedMilliseconds);
            return Task.CompletedTask;
        }

        private string GetConnectionString()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(Arguments.MasterConnectionString)
            {
                InitialCatalog = DatabaseInfo.Name
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }
    }
}
