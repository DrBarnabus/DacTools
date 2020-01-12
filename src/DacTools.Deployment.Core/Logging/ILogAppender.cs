﻿// Copyright (c) 2020 DrBarnabus

namespace DacTools.Deployment.Core.Logging
{
    public interface ILogAppender
    {
        void WriteTo(LogLevel logLevel, string message);
    }
}
