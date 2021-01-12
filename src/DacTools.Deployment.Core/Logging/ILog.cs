// Copyright (c) 2021 DrBarnabus

namespace DacTools.Deployment.Core.Logging
{
    public interface ILog
    {
        LogLevel LogLevel { get; set; }
        void Write(LogLevel logLevel, string format, params object[] args);

        void WriteRaw(LogLevel logLevel, string message);
        void AddLogAppender(ILogAppender logAppender);
    }
}
