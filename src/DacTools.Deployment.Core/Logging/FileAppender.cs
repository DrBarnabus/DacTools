// Copyright (c) 2022 DrBarnabus

using System;
using System.IO;

namespace DacTools.Deployment.Core.Logging
{
    public class FileAppender : ILogAppender
    {
        private readonly string _filePath;

        public FileAppender(string filePath)
        {
            _filePath = filePath;

            var logFile = new FileInfo(Path.GetFullPath(filePath));
            logFile.Directory?.Create();
            if (logFile.Exists)
                return;

            using (logFile.CreateText()) {}
        }

        public void WriteTo(LogLevel logLevel, string message)
        {
            try
            {
                string contents = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t\t{message}{Environment.NewLine}";
                File.AppendAllText(_filePath, contents);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
