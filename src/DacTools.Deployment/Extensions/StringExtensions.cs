// Copyright (c) 2022 DrBarnabus

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DacTools.Deployment.Extensions;

public static class StringExtensions
{
    private static readonly string[] Trues;
    private static readonly string[] Falses;

    static StringExtensions()
    {
        Trues = new[] { "1", "true" };
        Falses = new[] { "0", "false" };
    }

    public static bool IsTrue(this string value)
    {
        return Trues.Contains(value, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsFalse(this string value)
    {
        return Falses.Contains(value, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsValidFilePath(this string? filepath)
    {
        return !string.IsNullOrEmpty(filepath) && File.Exists(filepath);
    }

    public static bool IsSwitchArgument(this string? value)
    {
        return value != null
               && (value.StartsWith("-") || value.StartsWith("/"))
               && !Regex.Match(value, @"(-/)\w+:").Success;
    }

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

    public static bool IsHelp(this string singleArgument)
    {
        return singleArgument == "?" || singleArgument.IsSwitch("h") || singleArgument.IsSwitch("help") ||
               singleArgument.IsSwitch("?");
    }

    public static bool ArgumentRequiresValue(this string argument)
    {
        string[] booleanArguments =
        {
            "blacklist",
            "b"
        };

        if (argument.StartsWith("-"))
            argument = argument[1..];

        if (argument.StartsWith("/"))
            argument = argument[1..];

        var stringComparer = argument.Length == 1 ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
        return !booleanArguments.Contains(argument, stringComparer);
    }
}
