// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Logging;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core.Exceptions;

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

        public async Task Execute(Arguments arguments, CancellationToken cancellationToken)
        {
            if (arguments is null)
            {
                _helpWriter.Write();
                throw new FatalException("Arguments is null.");
            }

            if (arguments.IsVersion)
            {
                _versionWriter.Write(Assembly.GetExecutingAssembly());
                return;
            }

            if (arguments.IsHelp)
            {
                _helpWriter.Write();
                return;
            }

            // TODO: Add File Logging
            _log.AddLogAppender(new ConsoleAppender());

            if (!File.Exists(arguments.DacPacFilePath))
                throw new FatalException($"DacPac FilePath '{arguments.DacPacFilePath}' does not exist!", true);

            _log.Info("Using DacPac: {0}", arguments.DacPacFilePath);

            if (arguments.Threads == 0)
            {
                _log.Debug("Threads value was 0, so using default value of 1.");
                arguments.Threads = 1;
            }

            await _execCommand.Execute(cancellationToken);
        }
    }
}
