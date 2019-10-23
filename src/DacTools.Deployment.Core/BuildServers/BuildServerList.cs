using System;
using System.Collections.Generic;
using DacTools.Deployment.Core.BuildServers.Common;
using DacTools.Deployment.Core.Logging;

namespace DacTools.Deployment.Core.BuildServers
{
	public static class BuildServerList
	{
		private static List<IBuildServer> _supportedBuildServers;

		public static IEnumerable<IBuildServer> GetApplicableBuildServers(ILog log)
		{
			var buildServices = new List<IBuildServer>();

			foreach (var buildServer in _supportedBuildServers)
			{
				try
				{
					if (!buildServer.CanApplyToCurrentContext())
						continue;

					log.Info($"Applicable build agent found: '{buildServer.GetType().Name}'.");
					buildServices.Add(buildServer);
				}
				catch (Exception ex)
				{
					log.Warning($"Failed to check build server '{buildServer.GetType().Name}' because an exception occurred. Message: {ex.Message}");
				}
			}

			return buildServices;
		}

		public static void Init(ILog log)
		{
			_supportedBuildServers = new List<IBuildServer>
			{
				new AzurePipelines(log)
			};
		}
	}
}
