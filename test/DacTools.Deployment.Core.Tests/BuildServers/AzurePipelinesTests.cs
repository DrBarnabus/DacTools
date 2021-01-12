// Copyright (c) 2020 DrBarnabus

using DacTools.Deployment.Core.BuildServers;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
using Moq;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.BuildServers
{
    public class AzurePipelinesTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CanApplyToCurrentContextShouldReturnFalseWhenVariableIsNullOrEmpty(string variableValue)
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            environment.SetEnvironmentVariable("TF_BUILD", variableValue);

            // Act & Assert
            azurePipelines.CanApplyToCurrentContext().ShouldBe(false);
        }

        [Fact]
        public void CanApplyToCurrentContextShouldReturnTrueWhenVariableIsSet()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            environment.SetEnvironmentVariable("TF_BUILD", "True");

            // Act & Assert
            azurePipelines.CanApplyToCurrentContext().ShouldBe(true);
        }

        [Fact]
        public void GenerateLogIssueErrorMessageShouldReturnCorrectValue()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            // Act & Assert
            azurePipelines.GenerateLogIssueErrorMessage("error")
                .ShouldBe("##vso[task.logissue type=error;] error");
        }

        [Fact]
        public void GenerateLogIssueWarningMessageShouldReturnCorrectValue()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            // Act & Assert
            azurePipelines.GenerateLogIssueWarningMessage("warning")
                .ShouldBe("##vso[task.logissue type=warning;] warning");
        }

        [Theory]
        [InlineData(5, 10, 50)]
        [InlineData(4, 30, 13)]
        public void GenerateSetProgressMessageShouldReturnCorrectValue(int current, int total, int expected)
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            // Act & Assert
            azurePipelines.GenerateSetProgressMessage(current, total, "progress")
                .ShouldBe($"##vso[task.setprogress value={expected};] progress");
        }

        [Fact]
        public void GenerateSetStatusFailMessageShouldReturnCorrectValue()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            // Act & Assert
            azurePipelines.GenerateSetStatusFailMessage("status message")
                .ShouldBe("##vso[task.complete result=Failed;] status message");
        }

        [Fact]
        public void GenerateSetStatusSucceededWithIssuesMessageShouldReturnCorrectValue()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            // Act & Assert
            azurePipelines.GenerateSetStatusSucceededWithIssuesMessage("status message")
                .ShouldBe("##vso[task.complete result=SucceededWithIssues;] status message");
        }

        [Fact]
        public void GenerateSetStatusSucceededMessageShouldReturnCorrectValue()
        {
            // Setup
            var environment = new TestEnvironment();
            var log = new Mock<ILog>().Object;
            var azurePipelines = new AzurePipelines(environment, log);

            // Act & Assert
            azurePipelines.GenerateSetStatusSucceededMessage("status message")
                .ShouldBe("##vso[task.complete result=Succeeded;] status message");
        }
    }
}
