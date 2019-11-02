// Copyright (c) 2019 DrBarnabus

using System;
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

        public static bool IsValidFilePath(this string filepath) =>
            !string.IsNullOrEmpty(filepath) && File.Exists(filepath);

        public static bool IsSwitchArgument(this string value) =>
            value != null
            && (value.StartsWith("-") || value.StartsWith("/"))
            && !Regex.Match(value, @"/\w+:").Success;

        public static bool IsSwitch(this string value, string switchName)
        {
            if (value == null)
                return false;

            if (value.StartsWith("-"))
                value = value.Substring(1);

            if (value.StartsWith("/"))
                value = value.Substring(1);

            return string.Equals(switchName, value, switchName.Length == 1 ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsHelp(this string singleArgument) =>
            singleArgument == "?" || singleArgument.IsSwitch("h") || singleArgument.IsSwitch("help") || singleArgument.IsSwitch("?");

        public static bool ArgumentRequiresValue(this string argument, int argumentIndex)
        {
            var booleanArguments = new[]
            {
                "blacklist",
                "b"
            };

            bool argumentMightRequireValue = !booleanArguments.Contains(argument.Substring(1), StringComparer.OrdinalIgnoreCase);

            // If this is the first argument that might be the target DacPac filepath,
            // the argument starts with slash and we're running on an OS that supports paths with slashes, the argument does not require a value
            if (argumentMightRequireValue && argumentIndex == 0 && argument.StartsWith("/") && Path.DirectorySeparatorChar == '/' && argument.IsValidFilePath())
                return false;

            return argumentMightRequireValue;
        }
    }
}
