// Copyright (c) 2019 DrBarnabus

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment
{
    public class DeploymentExecutor : IDeploymentExecutor
    {
        private readonly ILog _log;
        private readonly IHelpWriter _helpWriter;
        private readonly IVersionWriter _versionWriter;
        private readonly IExecCommand _execCommand;

        public DeploymentExecutor(ILog log, IHelpWriter helpWriter, IVersionWriter versionWriter, IExecCommand execCommand)
        {
            _log = log;
            _helpWriter = helpWriter;
            _versionWriter = versionWriter;
            _execCommand = execCommand;
        }

        public async Task<int> Execute(Arguments arguments, CancellationToken cancellationToken)
        {
            try
            {
                if (arguments is null)
                {
                    _helpWriter.Write();
                    return 1;
                }

                if (arguments.IsVersion)
                {
                    _versionWriter.Write(Assembly.GetExecutingAssembly());
                    return 0;
                }

                if (arguments.IsHelp)
                {
                    _helpWriter.Write();
                    return 0;
                }

                // TODO: Add File Logging
                _log.AddLogAppender(new ConsoleAppender());

                if (!File.Exists(arguments.DacPacFilePath))
                    _log.Warning("The DacPac file '{0}' does not exist.", arguments.DacPacFilePath);
                else
                    _log.Info("Using DacPac: {0}", arguments.DacPacFilePath);

                await _execCommand.Execute(cancellationToken);
            }
            catch (Exception)
            {
                return 1;
            }

            return 0;
        }
    }
}
