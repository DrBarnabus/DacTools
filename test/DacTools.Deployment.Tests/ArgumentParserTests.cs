// Copyright (c) 2021 DrBarnabus

using System;
using System.Collections.Generic;
using DacTools.Deployment.Core.Exceptions;
using DacTools.Deployment.Core.Logging;
using Microsoft.SqlServer.Dac;
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
            result.LogFilePath.ShouldBeNull();
            result.LogLevel.ShouldBe(expectedLogLevel);
        }

        [Theory]
        [InlineData("/log C:\\Test\\Logging.log", "C:\\Test\\Logging.log")]
        [InlineData("/l C:\\Test\\Logging.log", "C:\\Test\\Logging.log")]
        [InlineData("-log C:\\Test\\Logging.log", "C:\\Test\\Logging.log")]
        [InlineData("-l C:\\Test\\Logging.log", "C:\\Test\\Logging.log")]
        public void ShouldSetTheCorrectLogFilePath(string arguments, string expectedLogFilePath)
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
            result.LogLevel.ShouldBe(LogLevel.Info);
            result.LogFilePath.ShouldBe(expectedLogFilePath);
        }

        [Theory]
        [InlineData("/verbosity invalid")]
        [InlineData("/v invalid")]
        [InlineData("-verbosity invalid")]
        [InlineData("-v invalid")]
        public void ShouldThrowAArgumentParsingExceptionWhenTheVerbosityValueCannotBeProcessed(string arguments)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<ArgumentParsingException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe("Could not parse Verbosity value 'invalid'.");
        }

        [Theory]
        [InlineData("/dacpac invalid")]
        [InlineData("/d invalid")]
        [InlineData("-dacpac invalid")]
        [InlineData("-d invalid")]
        public void ShouldThrowAArgumentParsingExceptionWhenTheDacPacFilePathValueIsNotAValidFilePath(string arguments)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<ArgumentParsingException>(() => argumentParser.ParseArguments(arguments))
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
        public void ShouldThrowAArgumentParsingExceptionWhenThreadsValueIsInvalid(string arguments, string exceptionMessage)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<ArgumentParsingException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe(exceptionMessage);
        }

        [Theory]
        [InlineData("/p:BlockOnPossibleDataLoss", false)]
        [InlineData("/p:BlockOnPossibleDataLoss false", false)]
        [InlineData("/p:BlockOnPossibleDataLoss true", true)]
        [InlineData("-p:BlockOnPossibleDataLoss", false)]
        [InlineData("-p:BlockOnPossibleDataLoss false", false)]
        [InlineData("-p:BlockOnPossibleDataLoss true", true)]
        public void ShouldSetDacDeployOptionsBlockOnPossibleDataLoss(string arguments, bool expectedValue)
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
            result.DacDeployOptions.BlockOnPossibleDataLoss.ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData("/p:DropIndexesNotInSource", false)]
        [InlineData("/p:DropIndexesNotInSource false", false)]
        [InlineData("/p:DropIndexesNotInSource true", true)]
        [InlineData("-p:DropIndexesNotInSource", false)]
        [InlineData("-p:DropIndexesNotInSource false", false)]
        [InlineData("-p:DropIndexesNotInSource true", true)]
        public void ShouldSetDacDeployOptionsDropIndexesNotInSource(string arguments, bool expectedValue)
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
            result.DacDeployOptions.DropIndexesNotInSource.ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData("/p:IgnorePermissions", true)]
        [InlineData("/p:IgnorePermissions false", false)]
        [InlineData("/p:IgnorePermissions true", true)]
        [InlineData("-p:IgnorePermissions", true)]
        [InlineData("-p:IgnorePermissions false", false)]
        [InlineData("-p:IgnorePermissions true", true)]
        public void ShouldSetDacDeployOptionsIgnorePermissions(string arguments, bool expectedValue)
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
            result.DacDeployOptions.IgnorePermissions.ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData("/p:IgnoreRoleMembership", true)]
        [InlineData("/p:IgnoreRoleMembership false", false)]
        [InlineData("/p:IgnoreRoleMembership true", true)]
        [InlineData("-p:IgnoreRoleMembership", true)]
        [InlineData("-p:IgnoreRoleMembership false", false)]
        [InlineData("-p:IgnoreRoleMembership true", true)]
        public void ShouldSetDacDeployOptionsIgnoreRoleMembership(string arguments, bool expectedValue)
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
            result.DacDeployOptions.IgnoreRoleMembership.ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData("/p:GenerateSmartDefaults", true)]
        [InlineData("/p:GenerateSmartDefaults false", false)]
        [InlineData("/p:GenerateSmartDefaults true", true)]
        [InlineData("-p:GenerateSmartDefaults", true)]
        [InlineData("-p:GenerateSmartDefaults false", false)]
        [InlineData("-p:GenerateSmartDefaults true", true)]
        public void ShouldSetDacDeployOptionsGenerateSmartDefaults(string arguments, bool expectedValue)
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
            result.DacDeployOptions.GenerateSmartDefaults.ShouldBe(expectedValue);
        }

        [Theory]
        [InlineData("/p:DropObjectsNotInSource", true)]
        [InlineData("/p:DropObjectsNotInSource false", false)]
        [InlineData("/p:DropObjectsNotInSource true", true)]
        [InlineData("-p:DropObjectsNotInSource", true)]
        [InlineData("-p:DropObjectsNotInSource false", false)]
        [InlineData("-p:DropObjectsNotInSource true", true)]
        public void ShouldSetDacDeployOptionsDropObjectsNotInSource(string arguments, bool expectedValue)
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
            result.DacDeployOptions.DropObjectsNotInSource.ShouldBe(expectedValue);
        }

        public static IEnumerable<object[]> CorrectDacDeployOptionsDoNotDropObjectTypesValues(string switchStart) =>
            new List<object[]>
            {
                new object[] { $"{switchStart}", new[] { ObjectType.Filegroups, ObjectType.Files, ObjectType.Logins, ObjectType.Permissions, ObjectType.RoleMembership, ObjectType.Users } },
                new object[] { $"{switchStart} Logins", new[] { ObjectType.Logins } },
                new object[] { $"{switchStart} Logins Users", new[] { ObjectType.Logins, ObjectType.Users } },
                new object[] { $"{switchStart} Logins,Users", new[] { ObjectType.Logins, ObjectType.Users } }
            };

        [Theory]
        [MemberData(nameof(CorrectDacDeployOptionsDoNotDropObjectTypesValues), "/p:DoNotDropObjectTypes")]
        [MemberData(nameof(CorrectDacDeployOptionsDoNotDropObjectTypesValues), "-p:DoNotDropObjectTypes")]
        public void ShouldSetTheCorrectDacDeployOptionsDoNotDropObjectTypesValues(string arguments, ObjectType[] expectedValues)
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
            result.DacDeployOptions.DoNotDropObjectTypes.Length.ShouldBe(expectedValues.Length);
            result.DacDeployOptions.DoNotDropObjectTypes.ShouldBe(expectedValues);
        }

        [Theory]
        [InlineData("/p:DoNotDropObjectTypes invalid")]
        [InlineData("/p:DoNotDropObjectTypes Users invalid")]
        [InlineData("/p:DoNotDropObjectTypes Users,invalid")]
        [InlineData("-p:DoNotDropObjectTypes invalid")]
        [InlineData("-p:DoNotDropObjectTypes Users invalid")]
        [InlineData("-p:DoNotDropObjectTypes Users,invalid")]
        public void ShouldThrowAArgumentParsingExceptionWhenTheObjectTypeValueCannotBeProcessed(string arguments)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<ArgumentParsingException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe("Could not parse ObjectType value of 'invalid'.");
        }

        [Theory]
        [InlineData("/variable:CustomSqlCmdVariable VariableValue", "CustomSqlCmdVariable", "VariableValue")]
        [InlineData("-variable:CustomSqlCmdVariable VariableValue", "CustomSqlCmdVariable", "VariableValue")]
        public void ShouldSetTheCorrectSqlCommandVariableValue(string arguments, string expectedVariableName, string expectedVariableValue)
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
            result.DacDeployOptions.SqlCommandVariableValues.ShouldContainKeyAndValue(expectedVariableName, expectedVariableValue);
        }

        public static IEnumerable<object[]> CorrectSqlCommandVariableValuesWhenMultipleSupplied(string switchStart) =>
            new List<object[]>
            {
                new object[] { $"{switchStart}variable:Variable1 Value1 {switchStart}variable:Variable2 Value2", new[] { "Variable1", "Variable2" }, new[] { "Value1", "Value2" } }
            };

        [Theory]
        [MemberData(nameof(CorrectSqlCommandVariableValuesWhenMultipleSupplied), "/")]
        [MemberData(nameof(CorrectSqlCommandVariableValuesWhenMultipleSupplied), "-")]
        public void ShouldSetTheCorrectSqlCommandVariableValuesWhenMultipleSupplied(string arguments, string[] expectedVariableNames, string[] expectedVariableValues)
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

            for (int i = 0; i < expectedVariableNames.Length; i++)
                result.DacDeployOptions.SqlCommandVariableValues.ShouldContainKeyAndValue(expectedVariableNames[i], expectedVariableValues[i]);
        }

        [Theory]
        [InlineData("/variable:DuplicateName DuplicateValue /variable:DuplicateName DuplicateValue")]
        [InlineData("-variable:DuplicateName DuplicateValue -variable:DuplicateName DuplicateValue")]
        public void ShouldThrowAArgumentParsingExceptionWhenADuplicateSqlCommandVariableNameIsProvided(string arguments)
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<ArgumentParsingException>(() => argumentParser.ParseArguments(arguments))
                .Message.ShouldBe("A value for Variable named 'DuplicateName' has been defined more than once.");
        }

        [Fact]
        public void ShouldThrowAArgumentParsingExceptionWhenTheArgumentCouldNotBeParsed()
        {
            var argumentParser = new ArgumentParser();
            Should.Throw<ArgumentParsingException>(() => argumentParser.ParseArguments("/InvalidArgument"))
                .Message.ShouldBe("Could not parse command line parameter '/InvalidArgument'.");
        }
    }
}
