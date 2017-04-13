using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlayGen.ITAlert.Simulation.Common;

namespace PlayGen.ITAlert.Simulation.Configuration
{
	public static class EdgeConfigExtensions
	{
		public static EdgeConfig Reciprocate(this EdgeConfig edgeConfig)
		{
			return new EdgeConfig(edgeConfig.Destination, edgeConfig.SourcePosition.OppositePosition(), edgeConfig.Source, edgeConfig.Archetype, edgeConfig.Weight, edgeConfig.Length);
		}
	}
}
