// Copyright (c) 2022 DrBarnabus

using System;
using System.Reflection;

namespace DacTools.Deployment
{
    public interface IVersionWriter
    {
        void Write(Assembly assembly);
        void WriteTo(Assembly assembly, Action<string> writeAction);
    }
}
