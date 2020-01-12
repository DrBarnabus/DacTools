// Copyright (c) 2020 DrBarnabus

using System;

namespace DacTools.Deployment.Core.Exceptions
{
    public class FatalException : Exception
    {
        public FatalException(string message, bool shouldLog = false)
            : base(message)
        {
            ShouldLog = shouldLog;
        }

        public FatalException(string message, Exception innerException, bool shouldLog = false)
            : base(message, innerException)
        {
            ShouldLog = shouldLog;
        }

        public bool ShouldLog { get; }
    }
}
