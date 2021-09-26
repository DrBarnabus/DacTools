// Copyright (c) 2021 DrBarnabus

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DacTools.Deployment.Core;
using DacTools.Deployment.Core.Exceptions;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Extensions;
using Microsoft.SqlServer.Dac;

namespace DacTools.Deployment
{
    public class ArgumentParser : IArgumentParser
    {
        public Arguments ParseArguments(string commandLineArguments)
        {
            string[] arguments = commandLineArguments
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            return ParseArguments(arguments);
        }

        public Arguments ParseArguments(string[] commandLineArguments)
        {
            if (commandLineArguments.Length == 0 || commandLineArguments.First().IsHelp())
                return new Arguments { IsHelp = true };

            var arguments = new Arguments();

            var switchesAndValues = CollectSwitchesAndValuesFromArguments(commandLineArguments);

            foreach (string? name in switchesAndValues.AllKeys)
            {
                string[]? values = switchesAndValues.GetValues(name);
                string? value = values?.FirstOrDefault();

                if (name.IsSwitch("version"))
                {
                    EnsureArgumentValueCount(values);
                    arguments.IsVersion = true;
                    continue;
                }

                if (name.IsSwitch("verbosity") || name.IsSwitch("v"))
                {
                    if (!Enum.TryParse<LogLevel>(value, true, out var result))
                        throw new ArgumentParsingException($"Could not parse Verbosity value '{value}'.");

                    arguments.LogLevel = result;
                    continue;
                }

                if (name.IsSwitch("dacpac") || name.IsSwitch("d"))
                {
                    EnsureArgumentValueCount(values);

                    if (value.IsValidFilePath())
                        arguments.DacPacFilePath = value;
                    else
                        throw new ArgumentParsingException($"Could not parse DacPac value '{value}'.");

                    continue;
                }

                if (name.IsSwitch("log") || name.IsSwitch("l"))
                {
                    EnsureArgumentValueCount(values);

                    arguments.LogFilePath = value;
                    continue;
                }

                if (name.IsSwitch("masterconnectionstring") || name.IsSwitch("S"))
                {
                    EnsureArgumentValueCount(values);

                    arguments.MasterConnectionString = value;
                    continue;
                }

                if (name.IsSwitch("databases") || name.IsSwitch("D"))
                {
                    if (values != null && values.Any())
                        foreach (string v in values)
                            if (!v.Contains(','))
                                arguments.AddDatabaseName(v);
                            else
                                foreach (string subValue in v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                    arguments.AddDatabaseName(subValue);

                    continue;
                }

                if (name.IsSwitch("blacklist") || name.IsSwitch("b"))
                {
                    if (value is null || value.IsTrue())
                        arguments.IsBlacklist = true;

                    continue;
                }

                if (name.IsSwitch("threads") || name.IsSwitch("t"))
                {
                    EnsureArgumentValueCount(values);

                    if (!int.TryParse(value, out int result))
                        throw new ArgumentParsingException($"Could not parse Threads value of '{value}'.");

                    arguments.Threads = result switch
                    {
                        -1  => Environment.ProcessorCount,
                        > 0 => result,
                        _   => throw new ArgumentParsingException($"Threads parameter must be either minus one or greater than zero. Parsed a value of '{value}'.")
                    };

                    continue;
                }

                if (name.IsSwitch("azpipelines"))
                {
                    if (value is null || value.IsTrue())
                        arguments.AzPipelines = true;

                    continue;
                }

                if (name.IsVariable(out string? variableName))
                {
                    if (values is { Length: > 1 })
                        throw new ArgumentParsingException($"A value for Variable named '{variableName}' has been defined more than once.");

                    arguments.DacDeployOptions.SqlCommandVariableValues.Add(variableName, value);
                    continue;
                }

                if (name.IsParameter("BlockOnPossibleDataLoss"))
                {
                    arguments.DacDeployOptions.BlockOnPossibleDataLoss = ParseBooleanParameter(values, false);
                    continue;
                }

                if (name.IsParameter("DropIndexesNotInSource"))
                {
                    arguments.DacDeployOptions.DropIndexesNotInSource = ParseBooleanParameter(values, false);
                    continue;
                }

                if (name.IsParameter("IgnorePermissions"))
                {
                    arguments.DacDeployOptions.IgnorePermissions = ParseBooleanParameter(values, true);
                    continue;
                }

                if (name.IsParameter("IgnoreRoleMembership"))
                {
                    arguments.DacDeployOptions.IgnoreRoleMembership = ParseBooleanParameter(values, true);
                    continue;
                }

                if (name.IsParameter("IgnoreFileAndLogFilePath"))
                {
                    arguments.DacDeployOptions.IgnoreFileAndLogFilePath = ParseBooleanParameter(values, true);
                    continue;
                }

                if (name.IsParameter("GenerateSmartDefaults"))
                {
                    arguments.DacDeployOptions.GenerateSmartDefaults = ParseBooleanParameter(values, true);
                    continue;
                }

                if (name.IsParameter("DropObjectsNotInSource"))
                {
                    arguments.DacDeployOptions.DropObjectsNotInSource = ParseBooleanParameter(values, true);
                    continue;
                }

                if (name.IsParameter("DoNotDropObjectTypes"))
                {
                    if (values != null && values.Any())
                    {
                        var temporaryObjectTypes = new List<ObjectType>();
                        foreach (string v in values)
                            if (!v.Contains(','))
                            {
                                if (Enum.TryParse<ObjectType>(v, true, out var result))
                                    temporaryObjectTypes.Add(result);
                                else
                                    throw new ArgumentParsingException($"Could not parse ObjectType value of '{v}'.");
                            }
                            else
                            {
                                foreach (string subValue in v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                    if (Enum.TryParse<ObjectType>(subValue, true, out var result))
                                        temporaryObjectTypes.Add(result);
                                    else
                                        throw new ArgumentParsingException($"Could not parse ObjectType value of '{subValue}'.");
                            }

                        arguments.DacDeployOptions.DoNotDropObjectTypes = temporaryObjectTypes.ToArray();
                    }

                    continue;
                }

                throw new ArgumentParsingException($"Could not parse command line parameter '{name}'.");
            }

            return arguments;
        }

        private static bool ParseBooleanParameter(IReadOnlyList<string>? values, bool defaultValue)
        {
            EnsureArgumentValueCount(values);
            string? value = values?.FirstOrDefault();

            if (value is null)
                return defaultValue;

            if (value.IsFalse())
                return false;

            if (value.IsTrue())
                return true;

            throw new InvalidOperationException($"Could not parse Boolean Parameter Value of '{value}'.");
        }

        private static void EnsureArgumentValueCount(IReadOnlyList<string>? values, int maxArguments = 1)
        {
            if (values != null && values.Count > maxArguments)
                throw new ArgumentParsingException($"Could not parse command line parameter '{values[maxArguments]}'.");
        }

        private static NameValueCollection CollectSwitchesAndValuesFromArguments(IReadOnlyList<string> namedArguments)
        {
            var switchesAndValues = new NameValueCollection(StringComparer.Ordinal);
            string? currentKey = null;
            bool argumentRequiresValue = false;

            foreach (string arg in namedArguments)
            {
                if (!argumentRequiresValue && arg.IsSwitchArgument())
                {
                    currentKey = arg;
                    argumentRequiresValue = arg.ArgumentRequiresValue();
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
