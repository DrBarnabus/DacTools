// Copyright (c) 2021 DrBarnabus

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DacTools.Deployment.Extensions
{
    public static class StringExtensions
    {
        private static readonly string[] Trues;
        private static readonly string[] Falses;

        static StringExtensions()
        {
            Trues = new[] { "1", "true" };
            Falses = new[] { "0", "false" };
        }

        public static bool IsTrue(this string value) => Trues.Contains(value, StringComparer.OrdinalIgnoreCase);

        public static bool IsFalse(this string value) => Falses.Contains(value, StringComparer.OrdinalIgnoreCase);

        public static bool IsValidFilePath(this string? filepath) =>
            !string.IsNullOrEmpty(filepath) && File.Exists(filepath);

        public static bool IsSwitchArgument(this string? value) =>
            value != null
            && (value.StartsWith("-") || value.StartsWith("/"))
            && !Regex.Match(value, @"(-/)\w+:").Success;

        public static bool IsSwitch(this string? value, string switchName)
        {
            if (value == null)
                return false;

            if (value.StartsWith("-"))
                value = value[1..];

            if (value.StartsWith("/"))
                value = value[1..];

            return string.Equals(switchName, value, switchName.Length == 1 ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsVariable(this string? value, [NotNullWhen(true)] out string? variableName)
        {
            variableName = null;

            if (value == null)
                return false;

            if (value.StartsWith("-") || value.StartsWith("/"))
                value = value[1..];

            var match = Regex.Match(value, "^variable:([^ ].*)$");
            if (!match.Success)
                return false;

            variableName = match.Groups[1].Value;
            return true;
        }

        public static bool IsParameter(this string? value, string parameterName)
        {
            if (value == null)
                return false;

            if (value.StartsWith("-p:"))
                value = value[3..];

            if (value.StartsWith("/p:"))
                value = value[3..];

            return string.Equals(parameterName, value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHelp(this string singleArgument) =>
            singleArgument == "?" || singleArgument.IsSwitch("h") || singleArgument.IsSwitch("help") || singleArgument.IsSwitch("?");

        public static bool ArgumentRequiresValue(this string argument)
        {
            string[] booleanArguments = {
                "blacklist",
                "b"
            };

            return !booleanArguments.Contains(argument, StringComparer.OrdinalIgnoreCase);
        }
    }
}
