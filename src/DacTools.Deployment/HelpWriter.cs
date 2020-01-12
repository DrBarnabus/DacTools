// Copyright (c) 2020 DrBarnabus

using System;
using System.Reflection;

namespace DacTools.Deployment
{
    public class HelpWriter : IHelpWriter
    {
        private readonly IVersionWriter _versionWriter;

        public HelpWriter(IVersionWriter versionWriter)
        {
            _versionWriter = versionWriter ?? throw new ArgumentNullException(nameof(versionWriter));
        }

        public void Write() => WriteTo(Console.WriteLine);

        public void WriteTo(Action<string> writeAction)
        {
            string version = string.Empty;
            _versionWriter.WriteTo(Assembly.GetExecutingAssembly(), v => version = v);

            string message = "DacTools.Deployment v" + version + @"
DacTools.Deployment is a tool that can be used to deploy DacPac files to Microsoft SQL Server. This tool expands upon DacFx by providing the ability to run multiple deployments to multiple databases simultaniously in parallel.

<Application> [options...]

    <Application>                   The name of this Application, such as DacTools.Deployment.exe.
    [options...]                    Any of the below options you want to add as command line arguments.

    /version                        Displays the current version of DacTools.Deployment
    /help, /h or /?                 Shows this Help Text
    /verbosity or /v                Set Verbosity Level (Debug, Info, Warn, Error, None). Default is Info

    /dacpac or /d                   The path to the .dacpac file to run the operation on.
    /masterconnectionstring or /S   The SQL Server Connection string for the master database on the server to deploy to.
    /databases or /D                A list of databases to either whitelist (default) or blacklist.
                                    The list can be provided as a set of either comma or space separated values.
    /blacklist or /b                Configures the tool to use the value of the  '/databases' option as a blacklist.
    /threads or /t                  Configures the maximum number of threads to use while deploying databases.
                                    If set to -1 then the value of Environment.ProcessorCount will be used.
";

            writeAction(message);
        }
    }
}
