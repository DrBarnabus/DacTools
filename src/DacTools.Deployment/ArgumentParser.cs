// Copyright (c) 2019 DrBarnabus

using DacTools.Deployment.Core;
using DacTools.Deployment.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace DacTools.Deployment
{
    public class ArgumentParser : IArgumentParser
    {
        public Arguments ParseArguments(string commandLineArguments)
        {
            var arguments = commandLineArguments
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            return ParseArguments(arguments);
        }

        public Arguments ParseArguments(string[] commandLineArguments)
        {
            if (commandLineArguments.Length == 0)
                return new Arguments { IsHelp = true };

            string firstArgument = commandLineArguments.First();

            if (firstArgument.IsHelp())
                return new Arguments { IsHelp = true };

            var arguments = new Arguments();

            var switchesAndValues = CollectSwitchesAndValuesFromArguments(commandLineArguments);

            foreach (string name in switchesAndValues.AllKeys)
            {
                var values = switchesAndValues.GetValues(name);
                string value = values?.FirstOrDefault();

                if (name.IsSwitch("version"))
                {
                    EnsureArgumentValueCount(values);
                    arguments.IsVersion = true;
                    continue;
                }

                if (name.IsSwitch("dacpac"))
                {
                    EnsureArgumentValueCount(values);

                    if (value.IsValidFilePath())
                        arguments.DacPacFilePath = value;
                    else
                        throw new WarningException($"Could not parse DacPac value '{value}'.");

                    continue;
                }

                if (name.IsSwitch("loglevel"))
                {
                    if (Enum.TryParse(value, true, out arguments.LogLevel))
                        throw new WarningException($"Could not parse LogLevel value '{value}'.");
                }

                string couldNotParseMessage = $"Could not parse command line parameter '{name}'.";

                throw new WarningException(couldNotParseMessage);
            }

            return arguments;
        }

        private static void EnsureArgumentValueCount(IReadOnlyList<string> values, int maxArguments = 1)
        {
            if (values != null && values.Count > maxArguments)
                throw new WarningException($"Could not parse command line parameter '{values[maxArguments]}'.");
        }

        private static NameValueCollection CollectSwitchesAndValuesFromArguments(IReadOnlyList<string> namedArguments)
        {
            var switchesAndValues = new NameValueCollection();
            string currentKey = null;
            bool argumentRequiresValue = false;

            for (int i = 0; i < namedArguments.Count; i++)
            {
                string arg = namedArguments[i];

                if (!argumentRequiresValue && arg.IsSwitchArgument())
                {
                    currentKey = arg;
                    argumentRequiresValue = arg.ArgumentRequiresValue(i);
                    switchesAndValues.Add(currentKey, null);
                }
                else if (currentKey != null)
                {
                    if (string.IsNullOrEmpty(switchesAndValues[currentKey]))
                        switchesAndValues[currentKey] = arg;
                    else
                        switchesAndValues.Add(currentKey, arg);

                    argumentRequiresValue = false;
                }
            }

            return switchesAndValues;
        }
    }
}
