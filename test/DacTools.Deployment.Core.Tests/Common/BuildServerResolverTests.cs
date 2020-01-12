// Copyright (c) 2020 DrBarnabus

using DacTools.Deployment.Core.BuildServers;
using DacTools.Deployment.Core.Common;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
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
            var buildServerResolver = new BuildServerResolver(new[] { new AzurePipelines(environment, log) }, log);

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
            var azurePipelines = new AzurePipelines(environment, log);
            var buildServerResolver = new BuildServerResolver(new[] { azurePipelines }, log);

            environment.SetEnvironmentVariable("TF_BUILD", "True");

            // Act
            var result = buildServerResolver.Resolve();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBe(azurePipelines);
        }
    }
}
