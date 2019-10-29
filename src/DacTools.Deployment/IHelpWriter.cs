// Copyright (c) 2019 DrBarnabus

using System;

namespace DacTools.Deployment
{
    public interface IHelpWriter
    {
        void Write();
        void WriteTo(Action<string> writeAction);
    }
}