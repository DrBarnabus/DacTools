// Copyright (c) 2019 DrBarnabus

using System;
using System.Collections.Generic;
using System.ComponentModel;
using DacTools.Deployment.Core.Logging;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Tests
{
    public class ArgumentParserTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("?")]
        [InlineData("/help")]
        [InlineData("-help")]
        [InlineData("/h")]
        [InlineData("-h")]
        [InlineData("/?")]
        [InlineData("-?")]
        public void ShouldSetIsHelpToTrue(string arguments)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeTrue();
            result.IsVersion.ShouldBeFalse();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldBeNull();
            result.IsBlacklist.ShouldBeFalse();
            result.Threads.ShouldBe(0);
            result.DatabaseNames.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("/version")]
        [InlineData("-version")]
        public void ShouldSetIsVersionToTrue(string arguments)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeFalse();
            result.IsVersion.ShouldBeTrue();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldBeNull();
            result.IsBlacklist.ShouldBeFalse();
            result.Threads.ShouldBe(0);
            result.DatabaseNames.ShouldBeEmpty();
        }

        public static IEnumerable<object[]> CorrectLogLevelTestData(string switchStart) =>
            new List<object[]>
            {
                new object[] { $"{switchStart}verbosity {LogLevel.None}", LogLevel.None },
                new object[] { $"{switchStart}v {LogLevel.None}", LogLevel.None },
                new object[] { $"{switchStart}verbosity {LogLevel.Error}", LogLevel.Error },
                new object[] { $"{switchStart}v {LogLevel.Error}", LogLevel.Error },
                new object[] { $"{switchStart}verbosity {LogLevel.Warn}", LogLevel.Warn },
                new object[] { $"{switchStart}v {LogLevel.Warn}", LogLevel.Warn },
                new object[] { $"{switchStart}verbosity {LogLevel.Info}", LogLevel.Info },
                new object[] { $"{switchStart}v {LogLevel.Info}", LogLevel.Info },
                new object[] { $"{switchStart}verbosity {LogLevel.Debug}", LogLevel.Debug },
                new object[] { $"{switchStart}v {LogLevel.Debug}", LogLevel.Debug }
            };

        [Theory]
        [MemberData(nameof(CorrectLogLevelTestData), "/")]
        [MemberData(nameof(CorrectLogLevelTestData), "-")]
        public void ShouldSetTheCorrectLogLevel(string arguments, LogLevel expectedLogLevel)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeFalse();
            result.IsVersion.ShouldBeFalse();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldBeNull();
            result.IsBlacklist.ShouldBeFalse();
            result.Threads.ShouldBe(0);
            result.DatabaseNames.ShouldBeEmpty();
            result.LogLevel.ShouldBe(expectedLogLevel);
        }

        [Theory]
        [InlineData("/verbosity invalid")]
        [InlineData("/v invalid")]
        [InlineData("-verbosity invalid")]
        [InlineData("-v invalid")]
        public void ShouldThrowAWarningExceptionWhenTheVerbosityValueCannotBeProcessed(string arguments)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<WarningException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe("Could not parse Verbosity value 'invalid'.");
        }

        [Theory]
        [InlineData("/dacpac invalid")]
        [InlineData("/d invalid")]
        [InlineData("-dacpac invalid")]
        [InlineData("-d invalid")]
        public void ShouldThrowAWarningExceptionWhenTheDacPacFilePathValueIsNotAValidFilePath(string arguments)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<WarningException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe("Could not parse DacPac value 'invalid'.");
        }

        [Theory]
        [InlineData("/masterconnectionstring value")]
        [InlineData("/S value")]
        [InlineData("-masterconnectionstring value")]
        [InlineData("-S value")]
        public void ShouldSetTheCorrectMasterConnectionString(string arguments)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeFalse();
            result.IsVersion.ShouldBeFalse();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldNotBeNull();
            result.MasterConnectionString.ShouldBe("value");
            result.IsBlacklist.ShouldBeFalse();
            result.Threads.ShouldBe(0);
            result.DatabaseNames.ShouldBeEmpty();
        }

        public static IEnumerable<object[]> CorrectDatabaseNamesTestData(string switchStart) =>
            new List<object[]>
            {
                new object[] { $"{switchStart} Value1", new[] { "Value1" } },
                new object[] { $"{switchStart}", Array.Empty<string>() },
                new object[] { $"{switchStart} Value1 Value2", new[] { "Value1", "Value2" } },
                new object[] { $"{switchStart} Value1,Value2", new[] { "Value1", "Value2" } }
            };

        [Theory]
        [MemberData(nameof(CorrectDatabaseNamesTestData), "/databases")]
        [MemberData(nameof(CorrectDatabaseNamesTestData), "-databases")]
        [MemberData(nameof(CorrectDatabaseNamesTestData), "/D")]
        [MemberData(nameof(CorrectDatabaseNamesTestData), "-D")]
        public void ShouldSetTheCorrectDatabasesNames(string arguments, string[] expectedValues)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeFalse();
            result.IsVersion.ShouldBeFalse();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldBeNull();
            result.IsBlacklist.ShouldBeFalse();
            result.Threads.ShouldBe(0);
            result.DatabaseNames.Count.ShouldBe(expectedValues.Length);
            result.DatabaseNames.ShouldBe(expectedValues);
        }

        [Theory]
        [InlineData("/blacklist", true)]
        [InlineData("/blacklist true", true)]
        [InlineData("/blacklist false", false)]
        [InlineData("/b", true)]
        [InlineData("/b true", true)]
        [InlineData("/b false", false)]
        [InlineData("-blacklist", true)]
        [InlineData("-blacklist true", true)]
        [InlineData("-blacklist false", false)]
        [InlineData("-b", true)]
        [InlineData("-b true", true)]
        [InlineData("-b false", false)]
        public void ShouldSetIsBlacklist(string arguments, bool expectedValue)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeFalse();
            result.IsVersion.ShouldBeFalse();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldBeNull();
            result.IsBlacklist.ShouldBe(expectedValue);
            result.Threads.ShouldBe(0);
            result.DatabaseNames.ShouldBeEmpty();
        }

        public static IEnumerable<object[]> CorrectThreadsTestData(string switchStart) =>
            new List<object[]>
            {
                new object[] { $"{switchStart} 1", 1 },
                new object[] { $"{switchStart} -1", Environment.ProcessorCount }
            };

        [Theory]
        [MemberData(nameof(CorrectThreadsTestData), "/threads")]
        [MemberData(nameof(CorrectThreadsTestData), "/t")]
        [MemberData(nameof(CorrectThreadsTestData), "-threads")]
        [MemberData(nameof(CorrectThreadsTestData), "-t")]
        public void ShouldSetThreads(string arguments, int expectedValue)
        {
            var argumentParser = new ArgumentParser();
            var result = argumentParser.ParseArguments(arguments);
            result.IsHelp.ShouldBeFalse();
            result.IsVersion.ShouldBeFalse();
            result.DacPacFilePath.ShouldBeNull();
            result.MasterConnectionString.ShouldBeNull();
            result.IsBlacklist.ShouldBeFalse();
            result.Threads.ShouldBe(expectedValue);
            result.DatabaseNames.ShouldBeEmpty();
        }

        public static IEnumerable<object[]> InvalidThreadsTestData(string switchStart) =>
            new List<object[]>
            {
                new object[] { $"{switchStart} 0", "Threads parameter must be either minus one or greater than zero. Parsed a value of '0'." },
                new object[] { $"{switchStart} -2", "Threads parameter must be either minus one or greater than zero. Parsed a value of '-2'." },
                new object[] { $"{switchStart}", "Could not parse Threads value of ''." },
                new object[] { $"{switchStart} a", "Could not parse Threads value of 'a'." },
                new object[] { $"{switchStart} 1 2", "Could not parse command line parameter '2'." }
            };

        [Theory]
        [MemberData(nameof(InvalidThreadsTestData), "/threads")]
        [MemberData(nameof(InvalidThreadsTestData), "/t")]
        [MemberData(nameof(InvalidThreadsTestData), "-threads")]
        [MemberData(nameof(InvalidThreadsTestData), "-t")]
        public void ShouldThrowAWarningExceptionWhenThreadsValueIsInvalid(string arguments, string exceptionMessage)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<WarningException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe(exceptionMessage);
        }

        [Fact]
        public void ShouldThrowAWarningExceptionWhenTheArgumentCouldNotBeParsed()
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<WarningException>(() => argumentParser.ParseArguments("/InvalidArgument"))
                .Message.ShouldBe("Could not parse command line parameter '/InvalidArgument'.");
        }
    }
}
