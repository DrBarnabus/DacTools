// Copyright (c) 2021 DrBarnabus

using DacTools.Deployment.Core.BuildServers;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.Common
{
    public class ActiveBuildServerTests
    {
        [Fact]
        public void InstanceShouldReturnNullWhenBuildServerNotAvailable()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log, GetMockArguments(false));
            var activeBuildServer = new ActiveBuildServer(new[] { azurePipelines }, log);

            // Act && Assert
            activeBuildServer.Instance.ShouldBeNull();
        }

        [Fact]
        public void IsActiveShouldReturnFalseWhenBuildServerNotAvailable()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log, GetMockArguments(false));
            var activeBuildServer = new ActiveBuildServer(new[] { azurePipelines }, log);

            // Act && Assert
            activeBuildServer.IsActive.ShouldBeFalse();
        }

        [Fact]
        public void InstanceShouldReturnTheCorrectInstanceWhenBuildServerIsAvailable()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log, GetMockArguments(true));
            var activeBuildServer = new ActiveBuildServer(new[] { azurePipelines }, log);

            // Act && Assert
            activeBuildServer.Instance.ShouldNotBeNull();
            activeBuildServer.Instance.ShouldBe(azurePipelines);
        }

        [Fact]
        public void IsActiveShouldReturnTrueWhenBuildServerIsAvailable()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log, GetMockArguments(true));
            var activeBuildServer = new ActiveBuildServer(new[] { azurePipelines }, log);

            // Act && Assert
            activeBuildServer.IsActive.ShouldBeTrue();
        }

        private static IOptions<Arguments> GetMockArguments(bool azPipelines)
        {
            var arguments = new Arguments { AzPipelines = azPipelines };
            var mock = new Mock<IOptions<Arguments>>();
            mock.Setup(o => o.Value).Returns(arguments);
            return mock.Object;
        }
    }
}
