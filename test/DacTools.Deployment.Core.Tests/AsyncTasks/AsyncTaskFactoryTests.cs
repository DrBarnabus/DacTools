// Copyright (c) 2022 DrBarnabus

using DacTools.Deployment.Core.AsyncTasks;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.AsyncTasks;

public class AsyncTaskFactoryTests
{
    [Fact]
    public void ShouldReturnTheCorrectInstance()
    {
        // Setup
        var arguments = new Arguments();
        var argumentsOptions = Options.Create(arguments);
        var log = new Mock<ILog>().Object;
        var buildServer = new Mock<IActiveBuildServer>().Object;
        var sut = new AsyncTaskFactory<TestAsyncTask2>(argumentsOptions, log, buildServer);

        // Act
        var result = sut.CreateAsyncTask();

        // Assert
        result.PublicArguments.ShouldBe(arguments);
    }
}
