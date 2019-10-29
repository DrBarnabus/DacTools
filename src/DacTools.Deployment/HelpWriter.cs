// Copyright (c) 2019 DrBarnabus

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
Used to deploy dacpac files to SQL Server. This tool has the ability to deploy a dacpac to multiple databases simultaniously.

DacTools.Deployment [options...]

    /version            Displays the current version of DacTools.Deployment
    /help, /h or /?     Shows this Help Text

    /dacpac             The path to the .dacpac file to run the operation on.
    /verbosity          Set Verbosity Level (Debug, Info, Warn, Error, None). Default is Info
";

            writeAction(message);
        }
    }
}
