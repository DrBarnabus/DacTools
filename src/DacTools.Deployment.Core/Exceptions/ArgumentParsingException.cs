// Copyright (c) 2020 DrBarnabus

using System;

namespace DacTools.Deployment.Core.Exceptions
{
    public class ArgumentParsingException : Exception
    {
        public ArgumentParsingException(string message)
            : base(message)
        {
        }
    }
}
