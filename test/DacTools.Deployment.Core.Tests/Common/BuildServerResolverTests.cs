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
    public class BuildServerResolverTests
    {
        [Fact]
        public void ShouldReturnNullWhenBuildServerDoesNotApply()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var buildServerResolver = new BuildServerResolver(new[] { new AzurePipelines(environment, log, GetMockArguments(false)) }, log);

            // Act
            var result = buildServerResolver.Resolve();

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public void ShouldReturnTheTheCorrectInstanceWhenBuildServerDoesApply()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log, GetMockArguments(true));
            var buildServerResolver = new BuildServerResolver(new[] { azurePipelines }, log);

            // Act
            var result = buildServerResolver.Resolve();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(azurePipelines);
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
