// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Dac;

namespace DacTools.Deployment.Core.AsyncTasks
{
    public class DacDeployAsyncTask : IAsyncTask
    {
        private readonly ILog _log;
        private readonly Arguments _arguments;
        private readonly DatabaseInfo _databaseInfo;
        private readonly ProgressUpdateDelegate _progressUpdateDelegate;

        public delegate void ProgressUpdateDelegate(bool successful);

        public DacDeployAsyncTask(ILog log, Arguments arguments, DatabaseInfo databaseInfo, ProgressUpdateDelegate progressUpdateDelegate)
        {
            _log = log;
            _arguments = arguments;
            _databaseInfo = databaseInfo;
            _progressUpdateDelegate = progressUpdateDelegate;
        }

        public void RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _log.Debug("DacPac Deployment Task Started for '{0}'.", _databaseInfo.ToString());

            var taskTimer = Stopwatch.StartNew();

            try
            {
                var dacPackage = DacPackage.Load(_arguments.DacPacFilePath, DacSchemaModelStorageType.Memory, FileAccess.Read);

                var dacServices = new DacServices(GetConnectionString());

                dacServices.Message += (sender, args) =>
                {
                    switch (args.Message.MessageType)
                    {
                        case DacMessageType.Error:
                            _log.Error("'{0}' DacServices Message - {1}", _databaseInfo.ToString(), args.Message);
                            break;
                        case DacMessageType.Warning:
                            _log.Warning("'{0}' DacServices Message - {1}", _databaseInfo.ToString(), args.Message);
                            break;
                        case DacMessageType.Message:
                            _log.Debug("'{0}' DacServices Message - {1}", _databaseInfo.ToString(), args.Message);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(args.Message.MessageType));
                    }
                };

                dacServices.ProgressChanged += (sender, args) => _log.Debug("'{0}' DacServices Progress Update ({2}:{3}) - {1}", _databaseInfo.ToString(), args.Message, args.Status, args.OperationId);

                // TODO: Make the DacDeployOptions Configurable
                dacServices.Deploy(dacPackage, _databaseInfo.Name, true, new DacDeployOptions(), cancellationToken);
            }
            catch (Exception ex)
            {
                _log.Error("DacPac Deployment Task for '{0}' failed after {1}ms with the following Error: {2}", _databaseInfo.ToString(), taskTimer.ElapsedMilliseconds, ex.Message);
                _progressUpdateDelegate(false);
            }

            _progressUpdateDelegate(true);
            _log.Debug("DacPac Deployment Task for '{0}' was completed successfully in {1}ms.", _databaseInfo.ToString(), taskTimer.ElapsedMilliseconds);
        }

        private string GetConnectionString()
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(_arguments.MasterConnectionString)
            {
                InitialCatalog = _databaseInfo.Name
            };

            return sqlConnectionStringBuilder.ConnectionString;
        }
    }
}
