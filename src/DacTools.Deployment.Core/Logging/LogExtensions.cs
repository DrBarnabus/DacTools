// Copyright (c) 2022 DrBarnabus

namespace DacTools.Deployment.Core.Logging;

public static class LogExtensions
{
    public static void Debug(this ILog log, string format, params object[] args)
    {
        log.Write(LogLevel.Debug, format, args);
    }

    public static void Warning(this ILog log, string format, params object[] args)
    {
        log.Write(LogLevel.Warn, format, args);
    }

    public static void Info(this ILog log, string format, params object[] args)
    {
        log.Write(LogLevel.Info, format, args);
    }

    public static void Error(this ILog log, string format, params object[] args)
    {
        log.Write(LogLevel.Error, format, args);
    }
}
