// Copyright (c) 2021 DrBarnabus

using System.Collections.Generic;
using DacTools.Deployment.Core.Common;

namespace DacTools.Deployment.Core.Tests.TestInfrastructure
{
    public class TestEnvironment : IEnvironment
    {
        private readonly Dictionary<string, string> _variableDictionary;

        public TestEnvironment()
        {
            _variableDictionary = new Dictionary<string, string>();
        }

        public string GetEnvironmentVariable(string variableName) => _variableDictionary.TryGetValue(variableName, out string value) ? value : null;

        public void SetEnvironmentVariable(string variableName, string value) => _variableDictionary[variableName] = value;
    }
}
