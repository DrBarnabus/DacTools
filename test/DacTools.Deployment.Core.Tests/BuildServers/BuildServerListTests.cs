using System.Linq;
using DacTools.Deployment.Core.BuildServers;
using DacTools.Deployment.Core.Logging;
using DacTools.Deployment.Core.Tests.TestInfrastructure;
using Moq;
using Shouldly;
using Xunit;

namespace DacTools.Deployment.Core.Tests.BuildServers
{
	public class BuildServerListTests
	{
		[Theory]
		[InlineData("True", 1)]
		[InlineData(null, 0)]
		public void GetApplicableBuildServersShouldReturnCorrectNumberOfValues(string variableValue, int expectedCount)
		{
			// Setup
			var environment = new TestEnvironment();
			var log = new Mock<ILog>().Object;
			BuildServerList.Init(environment, log);

			environment.SetEnvironmentVariable("TF_BUILD", variableValue);

			// Act
			var result = BuildServerList.GetApplicableBuildServers(log).ToList();

			// Assert
			result.ShouldNotBeNull();
			if (expectedCount == 1) result.ShouldNotBeEmpty();
			result.Count.ShouldBe(expectedCount);
		}
	}
}
