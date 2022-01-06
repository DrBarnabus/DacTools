// Copyright (c) 2022 DrBarnabus

using System.Collections.Generic;
using System.Linq;
using DacTools.Deployment.Core;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Tests;

public class HelpWriterTests
{
    [Fact]
    public void AllArgsAreMentionedInHelpText()
    {
        // Setup
        var versionWriter = new VersionWriter();
        var helpWriter = new HelpWriter(versionWriter);

        // For this test to work, we will need to keep the lookup up to date.
        var lookup = new Dictionary<string, string[]>
        {
            { "IsVersion", new[] { "/version" } },
            { "IsHelp", new[] { "/help", "/h ", "/?" } },
            { "DacPacFilePath", new[] { "/dacpac", "/d" } },
            { "MasterConnectionString", new[] { "/masterconnectionstring", "/S" } },
            { "IsBlacklist", new[] { "/blacklist", "/b" } },
            { "Threads", new[] { "/threads", "/t" } },
            { "DatabaseNames", new[] { "/databases", "/D" } },
            { "LogLevel", new[] { "/verbosity", "/v" } },
            { "LogFilePath", new[] { "/log", "/l" } },
            { "AzPipelines", new[] { "/azpipelines" } }
        };

        // Act
        string? helpText = null;
        helpWriter.WriteTo(s => helpText = s);
        helpText.ShouldNotBeNull();

        // Assert
        typeof(Arguments).GetFields()
            .Select(f => f.Name)
            .Where(f => f != "DacDeployOptions")
            .Where(f =>
            {
                lookup.ContainsKey(f).ShouldBeTrue();
                return lookup[f].Any(value => !helpText.Contains(value + (value.Length == 2 ? " " : "")));
            })
            .ShouldBeEmpty("One or More of the Switches were missing from the Help Text.");
    }
}
