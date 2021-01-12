// Copyright (c) 2021 DrBarnabus

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.DatabaseListGenerators;
using DacTools.Deployment.Core.Exceptions;
using DacTools.Deployment.Core.Logging;
using Microsoft.Extensions.Options;

namespace DacTools.Deployment
{
    public class ExecCommand : IExecCommand
    {
        private readonly Arguments _arguments;
        private readonly IBlacklistDatabaseListGenerator _blacklistDatabaseListGenerator;
        private readonly IDacPacDeployer _dacPacDeployer;
        private readonly ILog _log;
        private readonly IWhitelistDatabaseListGenerator _whitelistDatabaseListGenerator;

        public ExecCommand(ILog log, IWhitelistDatabaseListGenerator whitelistDatabaseListGenerator, IBlacklistDatabaseListGenerator blacklistDatabaseListGenerator, IDacPacDeployer dacPacDeployer, IOptions<Arguments> arguments)
        {
            _log = log;
            _whitelistDatabaseListGenerator = whitelistDatabaseListGenerator;
            _blacklistDatabaseListGenerator = blacklistDatabaseListGenerator;
            _dacPacDeployer = dacPacDeployer;
            _arguments = arguments.Value;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            _log.Info($"Running on {GetCurrentPlatform()}.");

            List<DatabaseInfo> databases;
            if (_arguments.IsBlacklist)
                databases = await _blacklistDatabaseListGenerator.GetDatabaseInfoListAsync(_arguments.DatabaseNames.ToList(), cancellationToken);
            else
                databases = await _whitelistDatabaseListGenerator.GetDatabaseInfoListAsync(_arguments.DatabaseNames.ToList(), cancellationToken);

            if (databases is null || !databases.Any())
                throw new FatalException($"{nameof(databases)} was null or empty.");

            _log.Info("Generated Database List: {0}", string.Join(", ", databases.Select(d => d.Name)));

            // TODO: Make the type of AsyncTask Soft as we might have multiple different "Operations"
            await _dacPacDeployer.DeployDacPac(databases, cancellationToken);
        }

        private static string GetCurrentPlatform() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "OSX" : "Unknown";
    }
}
