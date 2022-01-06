// Copyright (c) 2022 DrBarnabus

using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Exceptions;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment
{
    public class DeploymentExecutor : IDeploymentExecutor
    {
        private readonly IExecCommand _execCommand;
        private readonly IHelpWriter _helpWriter;
        private readonly ILog _log;
        private readonly IVersionWriter _versionWriter;

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

            // Configure Logging
            _log.AddLogAppender(new ConsoleAppender());

            if (arguments.LogFilePath != null)
                _log.AddLogAppender(new FileAppender(arguments.LogFilePath));

            if (string.IsNullOrEmpty(arguments.DacPacFilePath))
                throw new FatalException("DacPac FilePath was null or empty!", true);

            if (!File.Exists(arguments.DacPacFilePath))
                throw new FatalException($"DacPac FilePath '{arguments.DacPacFilePath}' does not exist!", true);

            // Continue with Deployment
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
