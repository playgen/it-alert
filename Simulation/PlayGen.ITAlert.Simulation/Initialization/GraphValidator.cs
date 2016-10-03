using System.Collections.Generic;
using System.Linq;
using PlayGen.ITAlert.Configuration;
using PlayGen.ITAlert.Simulation.Layout;

namespace PlayGen.ITAlert.Simulation.Initialization
{
	public static class GraphValidator
	{
		public static void ValidateConfig(List<NodeConfig> nodeConfigs, List<EdgeConfig> edgeConfigs)
		{
			#region initial validation

			if (nodeConfigs.Any() == false)
			{

				throw new LayoutException($"Graph invalid: No nodes specified.");
			}

			if (edgeConfigs.Any() == false)
			{
				throw new LayoutException($"Graph invalid: No edges specified.");
			}

			var nodeIds = new HashSet<int>();
			foreach (var nodeConfig in nodeConfigs)
			{
				if (nodeIds.Add(nodeConfig.Id) == false)
				{
					throw new LayoutException($"Graph invalid: Node {nodeConfig.Id} is duplicated.");
				}
				if (edgeConfigs.Any(ec => ec.Destination == nodeConfig.Id) == false)
				{
					throw new LayoutException($"Graph Invalid: Node {nodeConfig.Id} has no inbound connections.");
				}
				if (edgeConfigs.Any(ec => ec.Source == nodeConfig.Id) == false)
				{
					throw new LayoutException($"Graph Invalid: Node {nodeConfig.Id} has no outbound connections.");
				}
			}

			var edgeSet = new HashSet<EdgeConfig>();
			foreach (var edgeConfig in edgeConfigs)
			{
				if (edgeSet.Add(edgeConfig) == false)
				{
					throw new LayoutException($"Graph invalid: Edge with source {edgeConfig.Source} and destination {edgeConfig.Destination} is duplicated.");
				}
				if (nodeIds.Contains(edgeConfig.Source) == false)
				{
					throw new LayoutException($"Graph invalid: Source node {edgeConfig.Source} does not exist.");
				}
				if (nodeIds.Contains(edgeConfig.Destination) == false)
				{
					throw new LayoutException($"Graph invalid: Destination node {edgeConfig.Destination} does not exist.");
				}
			}

			#endregion
		}
	}
}
