// Copyright (c) 2020 DrBarnabus

namespace DacTools.Deployment.Core.Logging
{
    public interface ILog
    {
        LogLevel LogLevel { get; set; }
        void Write(LogLevel logLevel, string format, params object[] args);
        void AddLogAppender(ILogAppender logAppender);
    }
}
